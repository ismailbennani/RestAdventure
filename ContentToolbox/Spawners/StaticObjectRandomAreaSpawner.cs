using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Spawners;

/// <summary>
///     Spawn static objects in an area.
/// </summary>
/// <remarks>
///     <list type="bullet">
///         <item>All the instances of a static object are considered equal</item>
///         <item>Placement uses a uniform random distribution</item>
///     </list>
/// </remarks>
public class StaticObjectRandomAreaSpawner : RandomAreaSpawner<StaticObjectInstance>
{
    public StaticObjectRandomAreaSpawner(StaticObject staticObject, MapArea area, int maxCountInArea) : base(area, maxCountInArea)
    {
        StaticObject = staticObject;
    }

    public StaticObject StaticObject { get; }

    public override StaticObjectInstance Spawn(Location location) => new(StaticObject, location);

    public override int Count(IEnumerable<StaticObjectInstance> entities) => entities.Count(e => e.Object == StaticObject);
}
