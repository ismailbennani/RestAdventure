using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Items;

namespace RestAdventure.Game.Apis.Common.Dtos.Items;

/// <summary>
///     Item
/// </summary>
public class ItemDto : ItemMinimalDto
{
    /// <summary>
    ///     The description of the item
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     The weight of the item
    /// </summary>
    [Required]
    public required int Weight { get; init; }
}

static class ItemMappingExtensions
{
    public static ItemDto ToDto(this Item item) =>
        new()
        {
            Id = item.Id.Guid, Name = item.Name, Description = item.Description, Weight = item.Weight
        };
}
