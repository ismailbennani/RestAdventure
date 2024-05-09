using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Entities.StaticObjects;

public record StaticObjectId(Guid Guid) : ResourceId(Guid);

public class StaticObject : GameResource<StaticObjectId>
{
    public StaticObject() : base(new StaticObjectId(Guid.NewGuid()))
    {
    }

    public required string Name { get; init; }
    public string? Description { get; init; }
}
