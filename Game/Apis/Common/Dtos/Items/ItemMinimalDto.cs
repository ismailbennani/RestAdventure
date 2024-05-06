using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Items;

namespace RestAdventure.Game.Apis.Common.Dtos.Items;

/// <summary>
///     Item (minimal)
/// </summary>
public class ItemMinimalDto
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
}

static class ItemMinimalMappingExtensions
{
    public static ItemMinimalDto ToMinimalDto(this Item item) =>
        new()
        {
            Id = item.Id.Guid,
            Name = item.Name
        };
}
