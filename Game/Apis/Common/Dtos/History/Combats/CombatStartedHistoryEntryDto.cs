using RestAdventure.Core.History.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Combats;

/// <summary>
///     Combat started history entry
/// </summary>
public class CombatStartedHistoryEntryDto : CombatHistoryEntryDto
{
}

static class CombatStartedHistoryEntryMappingExtensions
{
    public static CombatStartedHistoryEntryDto ToDto(this CombatStartedHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            Turn = entry.Turn
        };
}
