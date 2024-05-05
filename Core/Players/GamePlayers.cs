using RestAdventure.Kernel.Security;

namespace RestAdventure.Core.Players;

public class GamePlayers
{
    readonly Dictionary<UserId, Player> _players = [];

    public GamePlayers(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public IEnumerable<Player> All => _players.Values;

    public Player RegisterPlayer(User user)
    {
        if (!_players.TryGetValue(user.Id, out Player? player))
        {
            player = new Player(user);
            _players[user.Id] = player;
        }

        return player;
    }

    public Player? GetPlayer(UserId userId) => _players.GetValueOrDefault(userId);
    public Player? GetPlayerByApiKey(ApiKey apiKey) => _players.Values.SingleOrDefault(p => p.User.ApiKey == apiKey);
}

public static class GamePlayersStateExtensions
{
    public static Player RequirePlayer(this GamePlayers state, UserId userId) => state.GetPlayer(userId) ?? throw new InvalidOperationException($"Could not find player {userId}");
}
