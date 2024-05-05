using RestAdventure.Core.Items;
using RestAdventure.Core.Maps;
using RestAdventure.Kernel.Security;

namespace RestAdventure.Core.Players;

/// <summary>
///     A user in a game
/// </summary>
public class Player
{
    readonly HashSet<MapLocationId> _discoveredLocations = [];
    readonly HashSet<ItemId> _discoveredItems = [];

    public Player(User user)
    {
        User = user;
    }

    public User User { get; }

    public void Discover(MapLocation location) => _discoveredLocations.Add(location.Id);
    public void Discover(Item item) => _discoveredItems.Add(item.Id);

    public bool HasDiscovered(MapLocation location) => _discoveredLocations.Contains(location.Id);
    public bool HasDiscovered(Item item) => _discoveredItems.Contains(item.Id);
}
