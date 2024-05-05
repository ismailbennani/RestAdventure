namespace RestAdventure.Core.Maps.Areas;

public class GameMapAreas
{
    readonly Dictionary<MapAreaId, MapArea> _areas = new();

    public IReadOnlyCollection<MapArea> All => _areas.Values;

    public void Register(MapArea area) => _areas[area.Id] = area;
    public MapArea? Get(MapAreaId areaId) => _areas.GetValueOrDefault(areaId);
}

public static class GameMapAreasExtensions
{
    public static MapArea Require(this GameMapAreas areas, MapAreaId areaId) => areas.Get(areaId) ?? throw new InvalidOperationException($"Could not find area {areaId}");
}
