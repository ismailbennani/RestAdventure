using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Items;

namespace RestAdventure.Game.Apis.Common.Dtos.Items;

/// <summary>
///     Item stack
/// </summary>
public class ItemStackDto
{
    /// <summary>
    ///     The item instance representing this stack
    /// </summary>
    [Required]
    public required ItemMinimalDto Item { get; init; }

    /// <summary>
    ///     The number of instances in this stack
    /// </summary>
    [Required]
    public int Count { get; init; }
}

static class ItemStackMappingExtensions
{
    public static ItemStackDto ToDto(this ItemStack stack) =>
        new()
        {
            Item = stack.Item.ToMinimalDto(),
            Count = stack.Count
        };
}
