using RestAdventure.Core.Items;

namespace RestAdventure.Core.Serialization.Entities;

public class InventorySnapshot
{
    public required IReadOnlyCollection<ItemInstanceStack> Stacks { get; init; }
    public int Weight { get; init; }

    public static InventorySnapshot Take(Inventory inventory) =>
        new()
        {
            Stacks = inventory.Stacks.Select(s => new ItemInstanceStack(s.ItemInstance, s.Count)).ToArray(),
            Weight = inventory.Weight
        };
}
