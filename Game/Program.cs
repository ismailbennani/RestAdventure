using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using NSwag;
using NSwag.Generation.Processors.Security;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps;
using RestAdventure.Game.Apis.AdminApi;
using RestAdventure.Game.Apis.GameApi;
using RestAdventure.Game.Apis.GameApi.Services.Characters;
using RestAdventure.Game.Apis.GameApi.Services.Game;
using RestAdventure.Game.Authentication;
using RestAdventure.Game.Settings;
using RestAdventure.Kernel.OpenApi;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Xtensive.Orm;

Assembly thisAssembly = typeof(Program).Assembly;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
ILoggerFactory loggerFactory = new SerilogLoggerFactory(Log.Logger);

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Domain domain = await builder.SetupPersistence(loggerFactory.CreateLogger("Persistence"), thisAssembly, typeof(TeamDbo).Assembly);

    {
        // TODO: remove this
        await using Session? session = await domain.OpenSessionAsync();
        await using TransactionScope? transaction = await session.OpenTransactionAsync();
        using SessionScope? _ = session.Activate();

        MapAreaDbo startingArea = new("Start");
        MapLocationDbo __ = new(startingArea, 0, 0);
        MapLocationDbo ___ = new(startingArea, 0, 1);

        __.ConnectedLocations.Add(___);

        transaction.Complete();
    }

    builder.Services.AddSerilog(
        (services, settings) => settings.WriteTo.Console(LogEventLevel.Verbose, "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} ({SourceContext}){NewLine}{Exception}")
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
    );
    builder.Services.AddControllers().AddJsonOptions(settings => settings.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

    SetupGameApiAuthentication(builder);
    SetupOpenApiDocuments(builder);

    builder.Services.AddOptions<GameSettings>();
    builder.Services.AddOptions<ServerSettings>();
    builder.Services.AddSingleton<TeamService>();
    builder.Services.AddSingleton<GameScheduler>();

    builder.Services.ConfigureCoreServices();

    WebApplication app = builder.Build();

    app.UseHttpsRedirection();

    app.UseOpenApi();
    app.UseSwaggerUi(settings => { settings.TagsSorter = "alpha"; });

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

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
            settings.Version = thisAssembly.GetName().Version!.ToString();

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
            settings.Version = thisAssembly.GetName().Version!.ToString();

            settings.OperationProcessors.Insert(0, new KeepOnlyControllersWithAttributeOperationProcessor(typeof(AdminApiAttribute)));
            settings.SchemaSettings.TypeNameGenerator = new TypeNameWithoutDtoGenerator(settings.SchemaSettings.TypeNameGenerator);
        }
    );
}

void StartScheduler(WebApplication app)
{
    GameScheduler scheduler = app.Services.GetRequiredService<GameScheduler>();
    scheduler.Start();
}
