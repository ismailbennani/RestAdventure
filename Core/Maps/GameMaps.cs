namespace RestAdventure.Core.Maps;

public class GameMaps
{
    readonly Dictionary<MapAreaId, MapArea> _areas = new();
    readonly Dictionary<MapLocationId, MapLocation> _locations = new();
    readonly List<(MapLocationId, MapLocationId)> _connections = [];

    public IReadOnlyCollection<MapArea> Areas => _areas.Values;
    public IReadOnlyCollection<MapLocation> Locations => _locations.Values;

    public MapArea CreateArea(string name)
    {
        MapArea area = new(name);
        _areas[area.Id] = area;

        return area;
    }

    public MapLocation CreateLocation(MapArea area, int x, int y)
    {
        MapLocation location = new(area, x, y);
        _locations[location.Id] = location;

        return location;
    }

    public MapArea? GetArea(MapAreaId areaId) => _areas.GetValueOrDefault(areaId);
    public MapLocation? GetLocation(MapLocationId locationId) => _locations.GetValueOrDefault(locationId);

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

    public bool IsConnected(MapLocation location1, MapLocation location2) =>
        _connections.Contains((location1.Id, location2.Id)) || _connections.Contains((location2.Id, location1.Id));
}

public static class GameMapsExtensions
{
    public static MapArea RequireArea(this GameMaps maps, MapAreaId areaId) => maps.GetArea(areaId) ?? throw new InvalidOperationException($"Could not find area {areaId}");

    public static MapLocation RequireLocation(this GameMaps maps, MapLocationId locationId) =>
        maps.GetLocation(locationId) ?? throw new InvalidOperationException($"Could not find location {locationId}");
}
