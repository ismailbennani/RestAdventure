using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Characters;

public record CharacterClassId(Guid Guid) : ResourceId(Guid);

public class CharacterClass : GameResource<CharacterClassId>
{
    public CharacterClass() : base(new CharacterClassId(Guid.NewGuid()))
    {
    }

    public required string Name { get; init; }
    public string? Description { get; init; }
    public required Location StartLocation { get; init; }
    public required IReadOnlyList<int> LevelCaps { get; init; }
}
