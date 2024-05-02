using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Game.Authentication;

class GameApiAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string AuthenticationScheme = "ApiKey";
    public string TokenHeaderName { get; set; } = "Authorization";
}

class GameApiAuthenticationHandler : AuthenticationHandler<GameApiAuthenticationOptions>
{
    readonly PlayerAuthenticationService _authenticationService;

    public GameApiAuthenticationHandler(
        PlayerAuthenticationService authenticationService,
        IOptionsMonitor<GameApiAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
        _authenticationService = authenticationService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.TokenHeaderName, out StringValues authTokens))
        {
            return Failure();
        }

        string? authToken = authTokens.FirstOrDefault();
        if (authToken == null || Guid.TryParse(authToken, out Guid authTokenGuid))
        {
            return Failure();
        }

        AuthenticationResult authenticationResult = await _authenticationService.AuthenticateAsync(authTokenGuid);
        if (!authenticationResult.IsSuccess)
        {
            return Failure();
        }

        return Success(authenticationResult.Session);
    }

    AuthenticateResult Success(PlayerSession session)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, session.PlayerId.ToString("D")),
            new Claim(ClaimTypes.Name, session.PlayerName)
        ];

        ClaimsIdentity claimsIdentity = new(claims, Scheme.Name);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }

    static AuthenticateResult Failure() => AuthenticateResult.Fail("Authentication failed");
}
