using RestAdventure.Core.Entities;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Spawners;

public abstract class EntitySpawner
{
    public abstract IEnumerable<GameEntity> Spawn(Location location);
}
