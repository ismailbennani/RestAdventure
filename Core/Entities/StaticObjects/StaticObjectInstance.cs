﻿using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities.StaticObjects;

public record StaticObjectInstanceId(Guid Guid) : GameEntityId(Guid);

public class StaticObjectInstance : GameEntity<StaticObjectInstanceId>, IStaticObjectInstance
{
    public StaticObjectInstance(StaticObject staticObject, Location location) : base(new StaticObjectInstanceId(Guid.NewGuid()), staticObject.Name, location)
    {
        Object = staticObject;
    }

    public StaticObject Object { get; }
}
