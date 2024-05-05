namespace RestAdventure.Core.Maps.Locations;

public class GameLocations
{
    readonly Dictionary<LocationId, Location> _locations = new();
    readonly List<(LocationId, LocationId)> _connections = [];

    public IReadOnlyCollection<Location> All => _locations.Values;

    public void Register(Location location) => _locations[location.Id] = location;
    public Location? Get(LocationId locationId) => _locations.GetValueOrDefault(locationId);

    public void Connect(Location location1, Location location2)
    {
        if (!_locations.ContainsKey(location1.Id))
        {
            throw new ArgumentException($"Location {location1} is not registered");
        }

        if (!_locations.ContainsKey(location2.Id))
        {
            throw new ArgumentException($"Location {location2} is not registered");
        }

        _connections.Add((location1.Id, location2.Id));
    }

    public IEnumerable<Location> ConnectedTo(Location location) =>
        _connections.Where(l => l.Item1 == location.Id || l.Item2 == location.Id)
            .Select(connection => connection.Item1 == location.Id ? _locations[connection.Item2] : _locations[connection.Item1]);

    public bool AreConnected(Location location1, Location location2) => _connections.Contains((location1.Id, location2.Id)) || _connections.Contains((location2.Id, location1.Id));
}

public static class GameMapLocationsExtensions
{
    public static Location Require(this GameLocations locations, LocationId locationId) =>
        locations.Get(locationId) ?? throw new InvalidOperationException($"Could not find location {locationId}");
}
