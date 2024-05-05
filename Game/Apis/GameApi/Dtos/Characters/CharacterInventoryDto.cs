using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Characters;
using RestAdventure.Game.Apis.GameApi.Dtos.Items;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Characters;

/// <summary>
///     Character inventory
/// </summary>
public class CharacterInventoryDto : InventoryDto
{
    /// <summary>
    ///     The total weight of the items in the inventory
    /// </summary>
    [Required]
    public int Weight { get; init; }
}

static class CharacterInventoryMappingExtensions
{
    public static CharacterInventoryDto ToDto(this CharacterInventory inventory) =>
        new()
        {
            Entries = inventory.Stacks.Select(e => e.ToDto()).ToArray(),
            Weight = inventory.Weight
        };
}
