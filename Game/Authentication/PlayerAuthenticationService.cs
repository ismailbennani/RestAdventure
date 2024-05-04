using RestAdventure.Core;
using RestAdventure.Core.Players;

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
            Player? player = _gameService.RequireGameState().Players.GetPlayerByApiKey(apiKey);
            if (player == null)
            {
                return AuthenticationResult.Failure();
            }

            session = new PlayerSession(player.Id, player.Name);
            _sessions[apiKey] = session;
        }

        session.LastActivityDate = DateTime.Now;

        return AuthenticationResult.Success(session);
    }
}
