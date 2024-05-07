using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using BaseGame;
using NSwag;
using NSwag.Generation.Processors.Security;
using RestAdventure.Core;
using RestAdventure.Core.Settings;
using RestAdventure.Core.StaticObjects;
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

    BaseGameContent baseGameContent = new();
    await baseGameContent.AddContentAsync(content);

    GameService gameService = app.Services.GetRequiredService<GameService>();
    GameState state = gameService.NewGame(content, new GameSettings());

    StaticObjectInstance appleTree1 = new(baseGameContent.Trees.AppleTree, baseGameContent.GeneratedMaps.Locations.First());
    await state.Entities.RegisterAsync(appleTree1);

    StaticObjectInstance appleTree2 = new(baseGameContent.Trees.AppleTree, baseGameContent.GeneratedMaps.Locations.First());
    await state.Entities.RegisterAsync(appleTree2);

    StaticObjectInstance pearTree = new(baseGameContent.Trees.PearTree, baseGameContent.GeneratedMaps.Locations.First());
    await state.Entities.RegisterAsync(pearTree);

    return state;
}

void StartScheduler(WebApplication app)
{
    GameScheduler scheduler = app.Services.GetRequiredService<GameScheduler>();
    scheduler.Start();
}
