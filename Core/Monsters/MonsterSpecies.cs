using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Monsters;

public record MonsterSpeciesId(Guid Guid) : ResourceId(Guid);

public class MonsterSpecies : GameResource<MonsterSpeciesId>
{
    public MonsterSpecies() : base(new MonsterSpeciesId(Guid.NewGuid()))
    {
    }

    public required MonsterFamily Family { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required int Health { get; init; }
    public required int Speed { get; init; }
    public required int Attack { get; init; }
}
