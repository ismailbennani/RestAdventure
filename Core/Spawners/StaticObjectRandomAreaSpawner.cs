using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Spawners;

public class StaticObjectRandomAreaSpawner : RandomAreaSpawner<StaticObjectInstance>
{
    public StaticObjectRandomAreaSpawner(StaticObject staticObject, MapArea area, int maxCount) : base(area, maxCount)
    {
        StaticObject = staticObject;
    }

    public StaticObject StaticObject { get; }

    public override StaticObjectInstance Spawn(Location location) => new(StaticObject, location);
    public override int Count(IEnumerable<StaticObjectInstance> entities) => entities.Count(e => e.Object == StaticObject);
}
