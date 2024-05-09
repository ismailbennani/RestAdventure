using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ExampleGame;
using NSwag;
using NSwag.Generation.Processors.Security;
using RestAdventure.Core;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Hosting;
using RestAdventure.Core.Players;
using RestAdventure.Core.Plugins;
using RestAdventure.Game.Apis.AdminApi;
using RestAdventure.Game.Apis.GameApi;
using RestAdventure.Game.Authentication;
using RestAdventure.Game.Services;
using RestAdventure.Game.Settings;
using RestAdventure.Kernel.OpenApi;
using RestAdventure.Kernel.Security;
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
    builder.Services.AddSingleton<GameSimulation>();

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
    ExampleGameScenarioBuilder exampleGameScenarioBuilder = new();
    Scenario scenario = exampleGameScenarioBuilder.Build();

    GameService gameService = app.Services.GetRequiredService<GameService>();
    GameState state = await gameService.NewGameAsync(scenario, new GameSettings());

    StaticObjectInstance pearTree = new(exampleGameScenarioBuilder.Gatherer.PearTree, exampleGameScenarioBuilder.GeneratedMaps.Locations.First());
    await state.Entities.AddAsync(pearTree);

    MonsterInstance petitPaw = new(exampleGameScenarioBuilder.Rattlings.PetitPaw, 1, exampleGameScenarioBuilder.GeneratedMaps.Locations.First());
    await state.Entities.AddAsync(petitPaw);

    MonsterInstance biggaud = new(exampleGameScenarioBuilder.Rattlings.Biggaud, 1, exampleGameScenarioBuilder.GeneratedMaps.Locations.First());
    await state.Entities.AddAsync(biggaud);

    Player player = await state.Players.RegisterPlayerAsync(new User(new UserId(Guid.NewGuid()), "PLAYER"));
    await state.Entities.AddAsync(new Character(player, exampleGameScenarioBuilder.CharacterClasses.Dealer, "Deadea"));
    await state.Entities.AddAsync(new Character(player, exampleGameScenarioBuilder.CharacterClasses.Knight, "Knikni"));

    return state;
}

void StartScheduler(WebApplication app)
{
    GameSimulation simulation = app.Services.GetRequiredService<GameSimulation>();
    simulation.Start();
}
