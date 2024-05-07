using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Monsters;

public record MonsterFamilyId(Guid Guid) : ResourceId(Guid);

public class MonsterFamily : GameResource<MonsterFamilyId>
{
    public MonsterFamily() : base(new MonsterFamilyId(Guid.NewGuid()))
    {
    }

    public required string Name { get; init; }
    public string? Description { get; init; }
}
