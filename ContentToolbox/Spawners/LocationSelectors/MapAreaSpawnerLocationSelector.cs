using RestAdventure.Core;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Spawners.LocationSelectors;

public class MapAreaSpawnerLocationSelector : SpawnerLocationSelector
{
    public required MapArea Area { get; init; }

    public override IEnumerable<Location> GetLocations(GameState state) => state.Content.Maps.Locations.InArea(Area);
}
