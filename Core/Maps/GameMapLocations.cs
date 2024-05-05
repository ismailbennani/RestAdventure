namespace RestAdventure.Core.Maps;

public class GameMapLocations
{
    readonly Dictionary<MapLocationId, MapLocation> _locations = new();
    readonly List<(MapLocationId, MapLocationId)> _connections = [];

    public IReadOnlyCollection<MapLocation> All => _locations.Values;

    public void Register(MapLocation location) => _locations[location.Id] = location;
    public MapLocation? Get(MapLocationId locationId) => _locations.GetValueOrDefault(locationId);

    public void Connect(MapLocation location1, MapLocation location2)
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

    public IEnumerable<MapLocation> GetConnectedTo(MapLocation location) =>
        _connections.Where(l => l.Item1 == location.Id || l.Item2 == location.Id)
            .Select(connection => connection.Item1 == location.Id ? _locations[connection.Item2] : _locations[connection.Item1]);

    public bool AreConnected(MapLocation location1, MapLocation location2) =>
        _connections.Contains((location1.Id, location2.Id)) || _connections.Contains((location2.Id, location1.Id));
}

public static class GameMapLocationsExtensions
{
    public static MapLocation Require(this GameMapLocations locations, MapLocationId locationId) =>
        locations.Get(locationId) ?? throw new InvalidOperationException($"Could not find location {locationId}");
}
