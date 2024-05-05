using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Items;

namespace RestAdventure.Game.Apis.Common.Dtos.Items;

/// <summary>
///     Inventory
/// </summary>
public class InventoryDto
{
    /// <summary>
    ///     The entries of the inventory
    /// </summary>
    [Required]
    public required IReadOnlyCollection<ItemStackDto> Entries { get; init; }

    /// <summary>
    ///     The total weight of the items in the inventory
    /// </summary>
    [Required]
    public int Weight { get; init; }
}

static class InventoryMappingExtensions
{
    public static InventoryDto ToDto(this IReadOnlyInventory inventory) =>
        new()
        {
            Entries = inventory.Stacks.Select(s => s.ToDto()).ToArray(),
            Weight = inventory.Weight
        };
}
