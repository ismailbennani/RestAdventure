namespace RestAdventure.Core.Maps;

public class GameMapState
{
    readonly Dictionary<Guid, MapArea> _areas = new();
    readonly Dictionary<Guid, MapLocation> _locations = new();
    readonly List<(Guid, Guid)> _connections = [];

    public GameMapState(GameState gameState)
    {
        GameState = gameState;
    }

    internal GameState GameState { get; }

    public IReadOnlyCollection<MapArea> Areas => _areas.Values;
    public IReadOnlyCollection<MapLocation> Locations => _locations.Values;

    public MapArea CreateArea(string name)
    {
        MapArea area = new(this, name);
        _areas[area.Id] = area;

        return area;
    }

    public MapLocation CreateLocation(MapArea area, int x, int y)
    {
        MapLocation location = new(area, x, y);
        _locations[location.Id] = location;

        return location;
    }

    public MapArea? GetArea(Guid areaId) => _areas.GetValueOrDefault(areaId);
    public MapLocation? GetLocation(Guid locationId) => _locations.GetValueOrDefault(locationId);

    public void ConnectLocations(MapLocation location1, MapLocation location2)
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

    public IEnumerable<MapLocation> GetAreaLocations(MapArea area) => _locations.Values.Where(l => l.Area == area);

    public IEnumerable<MapLocation> GetConnectedLocations(MapLocation location) =>
        _connections.Where(l => l.Item1 == location.Id || l.Item2 == location.Id)
            .Select(connection => connection.Item1 == location.Id ? _locations[connection.Item2] : _locations[connection.Item1]);
}

public static class GameMapStateExtensions
{
    public static MapArea RequireArea(this GameMapState state, Guid areaId) => state.GetArea(areaId) ?? throw new InvalidOperationException($"Could not find area {areaId}");

    public static MapLocation RequireLocation(this GameMapState state, Guid locationId) =>
        state.GetLocation(locationId) ?? throw new InvalidOperationException($"Could not find location {locationId}");
}
