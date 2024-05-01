using System.Reflection;
using System.Text.RegularExpressions;
using Core.Players;
using GameApi;
using Kernel.OpenApi;
using Serilog;
using Serilog.Extensions.Logging;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
ILoggerFactory loggerFactory = new SerilogLoggerFactory(Log.Logger);

try
{
    Assembly thisAssembly = typeof(Program).Assembly;

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    await builder.SetupPersistence(loggerFactory.CreateLogger("Persistence"), typeof(GameApiHostingExtensions).Assembly, typeof(PlayerDbo).Assembly);

    builder.Services.AddSerilog();
    builder.Services.AddControllers();

    builder.SetupGameApi();

    builder.Services.AddOpenApiDocument(
        settings =>
        {
            settings.Title = "Rest Adventure - API";
            settings.DocumentName = "api";
            settings.Version = thisAssembly.GetName().Version!.ToString();

            settings.OperationProcessors.Insert(0, new KeepOnlyControllersInAssemblyOperationProcessor(thisAssembly));
        }
    );


    WebApplication app = builder.Build();

    app.UseHttpsRedirection();

    app.UseOpenApi(
        settings =>
        {
            settings.PostProcess += (document, request) =>
            {
                Match match = OpenApiDocumentNameRegex().Match(request.Path);
                if (!match.Success)
                {
                    return;
                }

                string documentName = match.Groups["name"].Value;
                string? pathPrefix = documentName switch
                {
                    "game" => "/game",
                    "api" => null,
                    _ => null
                };

                document.BasePath = pathPrefix;
            };
        }
    );
    app.UseSwaggerUi(settings => { settings.TagsSorter = "alpha"; });

    app.MapGameApi("/game");

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

partial class Program
{
    [GeneratedRegex("swagger/(?<name>[^/]*)/swagger.json")]
    private static partial Regex OpenApiDocumentNameRegex();
}
