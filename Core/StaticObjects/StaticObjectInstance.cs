using RestAdventure.Core.Actions;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.StaticObjects;

public record StaticObjectInstanceId(Guid Guid) : GameEntityId(Guid);

public class StaticObjectInstance : GameEntity<StaticObjectInstanceId>, IGameEntityWithDisabled
{
    public StaticObjectInstance(StaticObject staticObject, Location location) : base(new StaticObjectInstanceId(Guid.NewGuid()), staticObject.Name, location)
    {
        Object = staticObject;
    }

    public StaticObject Object { get; }
    public bool Disabled { get; private set; }
    public void Enable() => Disabled = false;

    public void Disable() => Disabled = true;
}
