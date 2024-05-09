using RestAdventure.Core.Items;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Entities.Monsters;

public record MonsterFamilyId(Guid Guid) : ResourceId(Guid);

public class MonsterFamily : GameResource<MonsterFamilyId>
{
    public MonsterFamily() : base(new MonsterFamilyId(Guid.NewGuid()))
    {
    }

    public required string Name { get; init; }
    public string? Description { get; init; }

    /// <summary>
    ///     The items that all the species of this family can drop when they die
    /// </summary>
    public IReadOnlyCollection<ItemStack> Items { get; init; } = Array.Empty<ItemStack>();
}
