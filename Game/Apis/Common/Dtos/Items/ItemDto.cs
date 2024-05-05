using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Items;

namespace RestAdventure.Game.Apis.Common.Dtos.Items;

/// <summary>
///     Item
/// </summary>
public class ItemDto
{
    /// <summary>
    ///     The unique ID of the item
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the item
    /// </summary>
    [Required]
    public required string Name { get; init; }

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
