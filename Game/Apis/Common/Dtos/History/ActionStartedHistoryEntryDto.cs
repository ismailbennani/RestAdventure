using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Actions;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Action started history entry
/// </summary>
public class ActionStartedHistoryEntryDto : CharacterHistoryEntryDto
{
    /// <summary>
    ///     The name of the action that has started
    /// </summary>
    [Required]
    public required string ActionName { get; init; }
}

static class CharacterStartedInteractionHistoryEntryMappingExtensions
{
    public static ActionStartedHistoryEntryDto ToDto(this ActionStartedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            ActionName = entry.ActionName
        };
}
