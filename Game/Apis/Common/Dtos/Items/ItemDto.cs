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
}

static class ItemMappingExtensions
{
    public static ItemDto ToDto(this Item item) =>
        new()
        {
            Id = item.Id.Guid,
            Name = item.Name,
            Description = item.Description,
            ItemCategoryId = item.Category.Id.Guid,
            Weight = item.Weight
        };
}
