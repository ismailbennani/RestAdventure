using System.Reflection;
using Core.Players;
using NSwag;
using NSwag.Generation.Processors.Security;
using Serilog;
using Serilog.Extensions.Logging;
using Server.Authentication;
using Server.OpenApi;
using Server.Persistence;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
ILoggerFactory loggerFactory = new SerilogLoggerFactory(Log.Logger);

try
{
    Assembly thisAssembly = typeof(Program).Assembly;

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    await builder.SetupPersistence(loggerFactory.CreateLogger("Persistence"), typeof(PlayerDbo).Assembly);

    builder.Services.AddSerilog();

    builder.SetupApiKeyAuthentication();

    builder.Services.AddControllers();
    builder.Services.AddOpenApiDocument(
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

    WebApplication app = builder.Build();

    app.UseHttpsRedirection();

    app.UseOpenApi();
    app.UseSwaggerUi();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapDefaultControllerRoute();

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
