using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat.Old;
using RestAdventure.Core.History.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Combats;

/// <summary>
///     Combat ended history entry
/// </summary>
public class CombatEndedHistoryEntryDto : CombatHistoryEntryDto
{
    /// <summary>
    ///     The winner of the combat
    /// </summary>
    [Required]
    public required CombatSide Winner { get; init; }
}

static class CombatEndedHistoryEntryMappingExtensions
{
    public static CombatEndedHistoryEntryDto ToDto(this CombatEndedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            Turn = entry.Turn,
            Winner = entry.Winner
        };
}
