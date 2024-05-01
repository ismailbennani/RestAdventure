namespace Server.Authentication;

static class ApiKeyAuthenticationHostingExtensions
{
    public static void SetupApiKeyAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<PlayerAuthenticationService>();
        builder.Services.AddAuthentication(ApiKeyAuthenticationOptions.AuthenticationScheme)
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.AuthenticationScheme, options => { });
    }
}
