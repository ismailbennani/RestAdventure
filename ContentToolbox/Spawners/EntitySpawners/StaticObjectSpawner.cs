using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Spawners.EntitySpawners;

public class StaticObjectSpawner : EntitySpawner
{
    public required StaticObject StaticObject { get; init; }

    public override int Count(IEnumerable<GameEntity> entities) => entities.OfType<StaticObjectInstance>().Count(i => i.Object == StaticObject);

    public override IEnumerable<GameEntity> Spawn(Location location)
    {
        yield return new StaticObjectInstance(StaticObject, location);
    }
}
