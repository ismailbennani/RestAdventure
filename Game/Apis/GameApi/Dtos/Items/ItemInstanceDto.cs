using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Items;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Items;

/// <summary>
///     Item instance
/// </summary>
public class ItemInstanceDto
{
    /// <summary>
    ///     The unique ID of this instance
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    ///     The unique ID of the item corresponding to this instance
    /// </summary>
    [Required]
    public Guid ItemId { get; init; }
}

static class ItemInstanceMappingExtensions
{
    public static ItemInstanceDto ToDto(this ItemInstance itemInstance) =>
        new()
        {
            Id = itemInstance.Id.Guid,
            ItemId = itemInstance.Item.Id.Guid
        };
}
