using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character inventory changed history entry
/// </summary>
public class CharacterInventoryChangedHistoryEntryDto : CharacterHistoryEntryDto
{
    /// <summary>
    ///     The
    /// </summary>
    [Required]
    public required Guid ItemInstanceId { get; init; }

    /// <summary>
    ///     The
    /// </summary>
    [Required]
    public required Guid ItemId { get; init; }

    /// <summary>
    ///     The
    /// </summary>
    [Required]
    public required string ItemName { get; init; }

    /// <summary>
    ///     The
    /// </summary>
    [Required]
    public required int OldCount { get; init; }

    /// <summary>
    ///     The
    /// </summary>
    [Required]
    public required int NewCount { get; init; }
}

static class CharacterInventoryChangedHistoryEntryMappingExtensions
{
    public static CharacterInventoryChangedHistoryEntryDto ToDto(this EntityInventoryChangedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            ItemInstanceId = entry.ItemInstanceId.Guid,
            ItemId = entry.ItemId.Guid,
            ItemName = entry.ItemName,
            OldCount = entry.OldCount,
            NewCount = entry.NewCount
        };
}
