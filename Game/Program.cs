using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using RestAdventure.Core;
using RestAdventure.Core.Conditions.Characters;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Harvestables;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Settings;
using RestAdventure.Game.Apis.AdminApi;
using RestAdventure.Game.Apis.GameApi;
using RestAdventure.Game.Apis.GameApi.Services.Game;
using RestAdventure.Game.Authentication;
using RestAdventure.Game.Settings;
using RestAdventure.Kernel.OpenApi;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

Assembly thisAssembly = typeof(Program).Assembly;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
ILoggerFactory loggerFactory = new SerilogLoggerFactory(Log.Logger);

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog(
        (services, settings) => settings.WriteTo.Console(LogEventLevel.Verbose, "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} ({SourceContext}){NewLine}{Exception}")
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
    );

    builder.Services.AddControllers().AddJsonOptions(settings => settings.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

    builder.Services.AddMediatR(
        cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.RegisterServicesFromAssemblyContaining<GameService>();
        }
    );

    SetupGameApiAuthentication(builder);
    SetupOpenApiDocuments(builder);

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().DisallowCredentials()));
    }

    builder.Services.AddOptions<ServerSettings>();
    builder.Services.AddSingleton<GameScheduler>();

    builder.Services.ConfigureCoreServices();

    WebApplication app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseCors();
    }

    app.UseHttpsRedirection();

    app.UseOpenApi();
    app.UseSwaggerUi(settings => { settings.TagsSorter = "alpha"; });

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    await LoadGameAsync(app);
    StartScheduler(app);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

return;

void SetupGameApiAuthentication(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<PlayerAuthenticationService>();
    builder.Services.AddAuthentication(GameApiAuthenticationOptions.AuthenticationScheme)
        .AddScheme<GameApiAuthenticationOptions, GameApiAuthenticationHandler>(GameApiAuthenticationOptions.AuthenticationScheme, options => { });
}

void SetupOpenApiDocuments(WebApplicationBuilder builder)
{
    builder.Services.AddOpenApiDocument(
        settings =>
        {
            settings.Title = "Rest Adventure - Game API";
            settings.DocumentName = "game";

            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(thisAssembly.Location);
            settings.Version = fileVersionInfo.ProductVersion;

            settings.OperationProcessors.Insert(0, new KeepOnlyControllersWithAttributeOperationProcessor(typeof(GameApiAttribute)));
            settings.SchemaSettings.TypeNameGenerator = new TypeNameWithoutDtoGenerator(settings.SchemaSettings.TypeNameGenerator);

            settings.AddSecurity(
                "api-key",
                new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = OpenApiSecurityApiKeyLocation.Header
                }
            );

            settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("api-key"));
        }
    );

    builder.Services.AddOpenApiDocument(
        settings =>
        {
            settings.Title = "Rest Adventure - Admin API";
            settings.DocumentName = "admin";

            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(thisAssembly.Location);
            settings.Version = fileVersionInfo.ProductVersion;

            settings.OperationProcessors.Insert(0, new KeepOnlyControllersWithAttributeOperationProcessor(typeof(AdminApiAttribute)));
            settings.SchemaSettings.TypeNameGenerator = new TypeNameWithoutDtoGenerator(settings.SchemaSettings.TypeNameGenerator);
        }
    );
}

async Task<GameState> LoadGameAsync(WebApplication app)
{
    GameContent content = new();

    MapArea area = new() { Name = "Start" };
    content.Maps.Areas.Register(area);

    Location location1 = new() { Area = area, PositionX = 0, PositionY = 0 };
    Location location2 = new() { Area = area, PositionX = 0, PositionY = 1 };
    content.Maps.Locations.Register(location1);
    content.Maps.Locations.Register(location2);
    content.Maps.Locations.Connect(location1, location2);

    Item apple = new() { Name = "Apple", Description = "A delicious apple.", Weight = 1 };
    content.Items.Register(apple);

    Item pear = new() { Name = "Pear", Description = "A very delicious pear.", Weight = 1 };
    content.Items.Register(pear);

    Job gatherer = new() { Name = "Gatherer", Description = "Gather stuff", Innate = true, LevelsExperience = [2, 5, 10] };
    content.Jobs.Register(gatherer);

    Harvestable appleTree = new()
    {
        Name = "Apple Tree",
        Description = "A tree that has apples.",
        HarvestCondition = new CharacterJobCondition(gatherer),
        HarvestDuration = 10,
        Items = [new ItemStack(apple, 1)],
        Experience = [new JobExperienceStack(gatherer, 1)]
    };
    content.Harvestables.Register(appleTree);

    Harvestable pearTree = new()
    {
        Name = "Pear Tree",
        Description = "A tree that has pears.",
        HarvestCondition = new CharacterJobCondition(gatherer, 2),
        HarvestDuration = 10,
        Items = [new ItemStack(pear, 1)],
        Experience = [new JobExperienceStack(gatherer, 5)]
    };
    content.Harvestables.Register(pearTree);

    GameService gameService = app.Services.GetRequiredService<GameService>();
    GameState state = gameService.NewGame(content, new GameSettings());

    HarvestableInstance appleTreeInstance = new(appleTree, location1);
    await state.Entities.RegisterAsync(appleTreeInstance);

    HarvestableInstance pearTreeInstance = new(pearTree, location1);
    await state.Entities.RegisterAsync(pearTreeInstance);

    return state;
}

void StartScheduler(WebApplication app)
{
    GameScheduler scheduler = app.Services.GetRequiredService<GameScheduler>();
    scheduler.Start();
}
