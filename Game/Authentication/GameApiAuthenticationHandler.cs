using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using RestAdventure.Core.Players;

namespace RestAdventure.Game.Authentication;

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

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.TokenHeaderName, out StringValues authTokens))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        string? authToken = authTokens.FirstOrDefault();
        if (authToken == null || !Guid.TryParse(authToken, out Guid authTokenGuid))
        {
            return Task.FromResult(AuthenticateResult.Fail("Bad auth token"));
        }

        ApiKey apiKey = new(authTokenGuid);
        AuthenticationResult authenticationResult = _authenticationService.Authenticate(apiKey);
        if (!authenticationResult.IsSuccess)
        {
            return Task.FromResult(AuthenticateResult.Fail("Authentication failed"));
        }

        return Task.FromResult(Success(authenticationResult.Session));
    }

    AuthenticateResult Success(PlayerSession session)
    {
        List<Claim> claims =
        [
            new Claim(ClaimTypes.NameIdentifier, session.PlayerId.Guid.ToString("D")),
            new Claim(ClaimTypes.Name, session.PlayerName)
        ];

        ClaimsIdentity claimsIdentity = new(claims, Scheme.Name);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }
}
