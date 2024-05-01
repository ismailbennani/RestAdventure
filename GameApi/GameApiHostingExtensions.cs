using System.Reflection;
using GameApi.Authentication;
using Kernel.OpenApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace GameApi;

public static class GameApiHostingExtensions
{
    public static void SetupGameApi(this WebApplicationBuilder builder)
    {
        Assembly thisAssembly = typeof(GameApiHostingExtensions).Assembly;

        builder.Services.AddControllers().AddApplicationPart(thisAssembly);

        #region Authentication

        builder.Services.AddSingleton<PlayerAuthenticationService>();

        builder.Services.AddAuthentication(GameApiAuthenticationOptions.AuthenticationScheme)
            .AddScheme<GameApiAuthenticationOptions, GameApiAuthenticationHandler>(GameApiAuthenticationOptions.AuthenticationScheme, options => { });

        #endregion

        #region OpenApi

        builder.Services.AddOpenApiDocument(
            settings =>
            {
                settings.Title = "Rest Adventure - Game API";
                settings.DocumentName = "game";
                settings.Version = thisAssembly.GetName().Version!.ToString();

                settings.OperationProcessors.Insert(0, new KeepOnlyControllersInAssemblyOperationProcessor(thisAssembly));
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

        #endregion

    }

    public static void MapGameApi(this WebApplication app, string basePath = "/") =>
        app.MapWhen(
            context => context.Request.Path.StartsWithSegments(basePath),
            gameApiApp =>
            {
                gameApiApp.UsePathBase(basePath);
                gameApiApp.UseRouting();

                gameApiApp.UseAuthentication();
                gameApiApp.UseAuthorization();

                gameApiApp.UseEndpoints(endpoints => endpoints.MapControllers());

            }
        );
}
