using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Maps;

public class GameMaps
{
    public GameResourcesStore<MapAreaId, MapArea> Areas { get; } = new();
    public GameLocations Locations { get; } = new();
}
