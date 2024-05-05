using RestAdventure.Core.Items;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Maps.Locations;
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

    public event EventHandler<MapLocation>? LocationDiscovered;
    public event EventHandler<Item>? ItemDiscovered;

    public void Discover(MapLocation location)
    {
        _discoveredLocations.Add(location.Id);
        LocationDiscovered?.Invoke(this, location);
    }

    public void Discover(Item item)
    {
        _discoveredItems.Add(item.Id);
        ItemDiscovered?.Invoke(this, item);
    }

    public bool HasDiscovered(MapLocation location) => _discoveredLocations.Contains(location.Id);
    public bool HasDiscovered(Item item) => _discoveredItems.Contains(item.Id);

    public override string ToString() => $"{User}";
}
