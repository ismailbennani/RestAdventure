using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Items;

namespace RestAdventure.Game.Apis.Common.Dtos.Items;

/// <summary>
///     Item stack
/// </summary>
public class ItemInstanceStackDto
{
    /// <summary>
    ///     The item instance representing this stack
    /// </summary>
    [Required]
    public required ItemInstanceDto ItemInstance { get; init; }

    /// <summary>
    ///     The number of instances in this stack
    /// </summary>
    [Required]
    public int Count { get; init; }
}

static class ItemInstanceStackMappingExtensions
{
    public static ItemInstanceStackDto ToDto(this ItemInstanceStack stack) =>
        new()
        {
            ItemInstance = stack.ItemInstance.ToDto(),
            Count = stack.Count
        };
}
