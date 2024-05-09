using RestAdventure.Core.History.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Combats;

/// <summary>
///     Combat preparation started history entry
/// </summary>
public class CombatPreparationStartedHistoryEntryDto : CombatHistoryEntryDto
{
}

static class CombatPreparationStartedHistoryEntryMappingExtensions
{
    public static CombatPreparationStartedHistoryEntryDto ToDto(this CombatPreparationStartedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            Turn = entry.Turn
        };
}
