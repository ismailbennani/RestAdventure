namespace RestAdventure.Core.Players;

public class GamePlayersState
{
    readonly Dictionary<Guid, Player> _players = [];

    public GamePlayersState(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public Player RegisterPlayer(Guid playerId, string playerName)
    {
        if (!_players.TryGetValue(playerId, out Player? player))
        {
            player = new Player(playerId, playerName);
            _players[playerId] = player;
        }

        return player;
    }

    public Player? GetPlayer(Guid playerId) => _players.GetValueOrDefault(playerId);

    public Player? GetPlayerByApiKey(Guid apiKey) => _players.Values.SingleOrDefault(p => p.ApiKey == apiKey);
}

public static class GamePlayersStateExtensions
{
    public static Player RequirePlayer(this GamePlayersState state, Guid playerId) =>
        state.GetPlayer(playerId) ?? throw new InvalidOperationException($"Could not find player {playerId}");
}
