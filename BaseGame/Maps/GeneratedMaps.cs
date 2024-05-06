using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Harvestables;
using RestAdventure.Core.Maps.Locations;

namespace BaseGame.Maps;

public class GeneratedMaps
{
    public required IReadOnlyCollection<MapArea> Areas { get; init; }
    public required IReadOnlyCollection<Location> Locations { get; init; }
    public required IReadOnlyCollection<(Location, Location)> Connections { get; init; }
    public required IReadOnlyCollection<HarvestableInstance> Harvestables { get; init; }
}
