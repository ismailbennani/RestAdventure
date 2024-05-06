using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
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

    CharacterClass knight = new()
    {
        Name = "Knight",
        Description = "The Knight, the one who carries the weight of the world on their shoulders, or at least all the blows from enemies. "
                      + "Sturdy as a fortress, tough as nails, and about as subtle as a charging rhino.",
        LevelCaps = [2, 5, 10]
    };
    content.Characters.Classes.Register(knight);

    CharacterClass mage = new()
    {
        Name = "Mage",
        Description = "Behold, the Mage! Their power rivals that of the gods themselves, yet one swift breeze could knock them over. "
                      + "Fragile as a butterfly's wing, but with a mind as sharp as a wizard's hat.",
        LevelCaps = [2, 5, 10]
    };
    content.Characters.Classes.Register(mage);

    CharacterClass scout = new()
    {
        Name = "Scout",
        Description = "Presenting the Scout, the embodiment of speed and stealth, with a touch of sass thrown in for good measure. "
                      + "Quick as lightning and sneaky as a pickpocket in a crowded market, they dance through danger with all the grace of a cat on a hot tin roof. "
                      + "Sure, they might not bench-press boulders, but who needs muscles when you can outsmart your foes before they even know you're there? Fragile? Maybe. "
                      + "But they'll have you eating their dust faster than you can say 'gotcha'.",
        LevelCaps = [2, 5, 10]
    };
    content.Characters.Classes.Register(scout);

    CharacterClass dealer = new()
    {
        Name = "Dealer",
        Description = "Ah, the Dealer, not one for brawls but they'll haggle the pants off a troll. Their weapon of choice? The art of the deal. "
                      + "While others swing swords, they wield contracts and coin purses with finesse. Just don't expect them to throw down in a fistfight unless it's over prices.",
        LevelCaps = [2, 5, 10]
    };
    content.Characters.Classes.Register(dealer);

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

    Job gatherer = new() { Name = "Gatherer", Description = "Gather stuff", Innate = true, LevelCaps = [2, 5, 10] };
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
