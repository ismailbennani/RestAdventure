using RestAdventure.Core;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Spawners;

public abstract class SpawnerLocationSelector
{
    public abstract IEnumerable<Location> GetLocations(GameState state);
}
