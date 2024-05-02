using System.Reflection;
using Core.Players;
using GameApi.Authentication;
using Kernel.OpenApi;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using Serilog.Extensions.Logging;

Assembly thisAssembly = typeof(Program).Assembly;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
ILoggerFactory loggerFactory = new SerilogLoggerFactory(Log.Logger);

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    await builder.SetupPersistence(loggerFactory.CreateLogger("Persistence"), thisAssembly, typeof(PlayerDbo).Assembly);

    builder.Services.AddSerilog();
    builder.Services.AddControllers();

    SetupGameApiAuthentication(builder);

    SetupOpenApiDocument(builder);

    WebApplication app = builder.Build();

    app.UseHttpsRedirection();

    app.UseOpenApi();
    app.UseSwaggerUi(settings => { settings.TagsSorter = "alpha"; });

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

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

void SetupGameApiAuthentication(WebApplicationBuilder webApplicationBuilder)
{

    webApplicationBuilder.Services.AddSingleton<PlayerAuthenticationService>();
    webApplicationBuilder.Services.AddAuthentication(GameApiAuthenticationOptions.AuthenticationScheme)
        .AddScheme<GameApiAuthenticationOptions, GameApiAuthenticationHandler>(GameApiAuthenticationOptions.AuthenticationScheme, options => { });
}

void SetupOpenApiDocument(WebApplicationBuilder builder1)
{
    builder1.Services.AddOpenApiDocument(
        settings =>
        {
            settings.Title = "Rest Adventure - Game API";
            settings.DocumentName = "game";
            settings.Version = thisAssembly.GetName().Version!.ToString();

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
}
