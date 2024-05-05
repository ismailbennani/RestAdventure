using RestAdventure.Core;
using RestAdventure.Core.Players;
using RestAdventure.Kernel.Security;

namespace RestAdventure.Game.Authentication;

class PlayerAuthenticationService
{
    readonly GameService _gameService;
    readonly Dictionary<ApiKey, PlayerSession> _sessions = new();

    public PlayerAuthenticationService(GameService gameService)
    {
        _gameService = gameService;
    }

    public AuthenticationResult Authenticate(ApiKey apiKey)
    {
        if (!_sessions.TryGetValue(apiKey, out PlayerSession? session))
        {
            Player? playerState = _gameService.RequireGameState().Players.GetPlayerByApiKey(apiKey);
            if (playerState == null)
            {
                return AuthenticationResult.Failure();
            }

            session = new PlayerSession(playerState.User.Id, playerState.User.Name);
            _sessions[apiKey] = session;
        }

        session.LastActivityDate = DateTime.Now;

        return AuthenticationResult.Success(session);
    }
}
