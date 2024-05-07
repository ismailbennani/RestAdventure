using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Maps.Areas;

public class GameMapAreas : GameResourcesStore<MapAreaId, MapArea>
{
}

public static class GameMapAreasExtensions
{
    public static MapArea Require(this GameMapAreas areas, MapAreaId areaId) => areas.Get(areaId) ?? throw new InvalidOperationException($"Could not find area {areaId}");
}
