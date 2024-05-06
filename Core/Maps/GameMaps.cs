using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Harvestables;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Maps;

public class GameMaps
{
    public GameMapAreas Areas { get; } = new();
    public GameLocations Locations { get; } = new();
    public GameHarvestableInstances Harvestables { get; } = new();
}
