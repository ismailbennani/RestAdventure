using RestAdventure.Core.Items;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Entities.Monsters;

public record MonsterSpeciesId(Guid Guid) : ResourceId(Guid);

public class MonsterSpecies : GameResource<MonsterSpeciesId>
{
    public MonsterSpecies() : base(new MonsterSpeciesId(Guid.NewGuid()))
    {
    }

    public required MonsterFamily Family { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }

    /// <summary>
    ///     The items that the monsters of the species can drop when they die
    /// </summary>
    public IReadOnlyCollection<ItemStack> Items { get; init; } = Array.Empty<ItemStack>();

    /// <summary>
    ///     The base experience that the monsters of the species give to the characters that killed them
    /// </summary>
    public required int Experience { get; init; }

    public required int Health { get; init; }
    public required int Speed { get; init; }
    public required int Attack { get; init; }
}
