using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Actions;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Characters;

/// <summary>
///     Action ended history entry
/// </summary>
public class ActionEndedHistoryEntryDto : CharacterHistoryEntryDto
{
    /// <summary>
    ///     The name of the action that has ended
    /// </summary>
    [Required]
    public required string ActionName { get; init; }
}

static class CharacterEndedInteractionHistoryEntryMappingExtensions
{
    public static ActionEndedHistoryEntryDto ToDto(this ActionEndedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            ActionName = entry.ActionName
        };
}
