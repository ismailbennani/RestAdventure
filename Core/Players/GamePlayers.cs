using MediatR;
using RestAdventure.Core.Players.Notifications;
using RestAdventure.Kernel.Security;

namespace RestAdventure.Core.Players;

public class GamePlayers : IDisposable
{
    readonly IPublisher _publisher;
    readonly Dictionary<UserId, Player> _players = [];

    public GamePlayers(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public IEnumerable<Player> All => _players.Values;

    public async Task<Player> RegisterPlayerAsync(User user)
    {
        if (!_players.TryGetValue(user.Id, out Player? player))
        {
            player = new Player(user);
            _players[user.Id] = player;

            player.Knowledge.Discovered += (_, resource) => _publisher.Publish(new PlayerDiscoveredResource { Player = player, Resource = resource }).Wait();

            await _publisher.Publish(new PlayerJoined { Player = player });
        }

        return player;
    }

    public Player? GetPlayer(UserId userId) => _players.GetValueOrDefault(userId);
    public Player? GetPlayerByApiKey(ApiKey apiKey) => _players.Values.SingleOrDefault(p => p.User.ApiKey == apiKey);

    public void Dispose()
    {
        foreach (Player player in _players.Values)
        {
            player.Dispose();
        }
        _players.Clear();

        GC.SuppressFinalize(this);
    }
}

public static class GamePlayersStateExtensions
{
    public static Player RequirePlayer(this GamePlayers state, UserId userId) => state.GetPlayer(userId) ?? throw new InvalidOperationException($"Could not find player {userId}");
}
