using RestAdventure.Core;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Spawners.LocationSelectors;

public class WholeMapSpawnerLocationSelector : SpawnerLocationSelector
{
    public override IEnumerable<Location> GetLocations(GameState state) => state.Content.Maps.Locations;
}
