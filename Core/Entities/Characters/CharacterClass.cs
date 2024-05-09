using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Entities.Characters;

public record CharacterClassId(Guid Guid) : ResourceId(Guid);

public class CharacterClass : GameResource<CharacterClassId>
{
    public CharacterClass() : base(new CharacterClassId(Guid.NewGuid()))
    {
    }

    public required string Name { get; init; }
    public string? Description { get; init; }
    public required Location StartLocation { get; init; }
    public required int Health { get; init; }
    public required int Speed { get; init; }
    public required int Attack { get; init; }
    public required IReadOnlyList<int> LevelCaps { get; init; }

    public override string ToString() => Name;
}
