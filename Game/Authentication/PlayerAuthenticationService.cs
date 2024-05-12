using System.Collections.Concurrent;
using RestAdventure.Core;
using RestAdventure.Core.Players;
using RestAdventure.Kernel.Security;

namespace RestAdventure.Game.Authentication;

class PlayerAuthenticationService
{
    readonly GameService _gameService;
    readonly ConcurrentDictionary<ApiKey, PlayerSession> _sessions = new();

    public PlayerAuthenticationService(GameService gameService)
    {
        _gameService = gameService;
    }

    public AuthenticationResult Authenticate(ApiKey apiKey)
    {
        Player? playerState = _gameService.RequireGame().Players.GetPlayerByApiKey(apiKey);
        if (playerState == null)
        {
            return AuthenticationResult.Failure();
        }

        PlayerSession session = _sessions.GetOrAdd(apiKey, _ => new PlayerSession(playerState.User.Id, playerState.User.Name));
        session.LastActivityDate = DateTime.Now;

        return AuthenticationResult.Success(session);
    }
}
