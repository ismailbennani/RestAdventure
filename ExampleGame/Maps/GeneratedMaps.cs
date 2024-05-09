using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Spawners;

namespace ExampleGame.Maps;

public class GeneratedMaps
{
    public required IReadOnlyCollection<MapArea> Areas { get; init; }
    public required IReadOnlyCollection<Location> Locations { get; init; }
    public required IReadOnlyCollection<(Location, Location)> Connections { get; init; }
    public required IReadOnlyCollection<Spawner> Spawners { get; init; }
}
