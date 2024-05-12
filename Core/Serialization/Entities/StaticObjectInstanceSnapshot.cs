using RestAdventure.Core.Entities.StaticObjects;

namespace RestAdventure.Core.Serialization.Entities;

public class StaticObjectInstanceSnapshot : GameEntitySnapshot<StaticObjectInstanceId>
{
    public StaticObjectInstanceSnapshot(StaticObjectInstanceId id) : base(id)
    {
    }

    public required StaticObject Object { get; init; }

    public static StaticObjectInstanceSnapshot Take(StaticObjectInstance instance) =>
        new(instance.Id)
        {
            Team = instance.Team == null ? null : TeamSnapshot.Take(instance.Team),
            Name = instance.Name,
            Location = instance.Location,
            Busy = instance.Busy,
            Object = instance.Object
        };
}
