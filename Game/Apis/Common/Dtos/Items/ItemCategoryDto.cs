using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Items;

namespace RestAdventure.Game.Apis.Common.Dtos.Items;

/// <summary>
///     Item category
/// </summary>
public class ItemCategoryDto
{
    /// <summary>
    ///     The id of the category
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the category
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class ItemCategoryMappingExtensions
{
    public static ItemCategoryDto ToDto(this ItemCategory category) =>
        new()
        {
            Id = category.Id.Guid,
            Name = category.Name
        };
}
