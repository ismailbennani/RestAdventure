namespace RestAdventure.Core.Players;

public class GamePlayersState
{
    readonly Dictionary<PlayerId, Player> _players = [];

    public GamePlayersState(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public IEnumerable<Player> All => _players.Values;

    public Player RegisterPlayer(PlayerId playerId, string playerName)
    {
        if (!_players.TryGetValue(playerId, out Player? player))
        {
            player = new Player(playerId, playerName);
            _players[playerId] = player;
        }

        return player;
    }

    public Player? GetPlayer(PlayerId playerId) => _players.GetValueOrDefault(playerId);

    public Player? GetPlayerByApiKey(ApiKey apiKey) => _players.Values.SingleOrDefault(p => p.ApiKey == apiKey);
}

public static class GamePlayersStateExtensions
{
    public static Player RequirePlayer(this GamePlayersState state, PlayerId playerId) =>
        state.GetPlayer(playerId) ?? throw new InvalidOperationException($"Could not find player {playerId}");
}
