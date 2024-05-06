using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Characters;
using RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character performed action
/// </summary>
public class CharacterPerformedActionHistoryEntryDto : CharacterHistoryEntryDto
{
    /// <summary>
    ///     The action that has been performed
    /// </summary>
    [Required]
    public required CharacterActionDto Action { get; init; }

    /// <summary>
    ///     Has the action been successful
    /// </summary>
    [Required]
    public required bool Success { get; init; }

    /// <summary>
    ///     The reason of the failure
    /// </summary>
    public required string? FailureReason { get; init; }
}

static class CharacterPerformedActionHistoryEntryMappingExtensions
{
    public static CharacterPerformedActionHistoryEntryDto ToDto(this CharacterPerformedActionHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            Action = entry.Action.ToDto(),
            Success = entry.Success,
            FailureReason = entry.FailureReason
        };
}
