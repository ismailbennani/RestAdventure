using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Items;

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
}
