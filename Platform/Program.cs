using System.Reflection;
using Kernel.OpenApi;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Serilog;
using Serilog.Extensions.Logging;

Assembly thisAssembly = typeof(Program).Assembly;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
ILoggerFactory loggerFactory = new SerilogLoggerFactory(Log.Logger);

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog();

    await builder.SetupPersistence(loggerFactory.CreateLogger("Persistence"), thisAssembly);
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(
            options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options);
                options.Scope.Add("platform");
            }
        );

    builder.Services.AddControllers();

    SetupOpenApiDocument(builder);

    WebApplication app = builder.Build();

    app.UseHttpsRedirection();

    if (app.Environment.IsDevelopment())
    {
        app.UseOpenApi();
        app.UseSwaggerUi(settings => { settings.TagsSorter = "alpha"; });
    }

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

void SetupOpenApiDocument(WebApplicationBuilder builder)
{
    builder.Services.AddOpenApiDocument(
        settings =>
        {
            settings.Title = "Rest Adventure - Platform API";
            settings.DocumentName = "platform";
            settings.Version = thisAssembly.GetName().Version!.ToString();

            settings.SchemaSettings.TypeNameGenerator = new TypeNameWithoutDtoGenerator(settings.SchemaSettings.TypeNameGenerator);
        }
    );
}
