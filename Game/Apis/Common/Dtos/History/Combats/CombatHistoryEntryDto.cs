using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RestAdventure.Core.History.Combats;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Combats;

/// <summary>
///     Combat history entry
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type", IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(CombatPreparationStartedHistoryEntryDto), "preparation-started")]
[JsonDerivedType(typeof(CombatStartedHistoryEntryDto), "started")]
[JsonDerivedType(typeof(CombatEntityJoinedHistoryEntryDto), "entity-joined")]
[JsonDerivedType(typeof(CombatEntityLeftHistoryEntryDto), "entity-left")]
[JsonDerivedType(typeof(CombatEndedHistoryEntryDto), "ended")]
public class CombatHistoryEntryDto
{
    /// <summary>
    ///     The tick at which the event happened
    /// </summary>
    [Required]
    public required long Tick { get; init; }

    /// <summary>
    ///     The turn at which the event happened
    /// </summary>
    [Required]
    public required long Turn { get; init; }
}

static class CombatHistoryEntryMappingExtensions
{
    public static CombatHistoryEntryDto ToDto(this CombatHistoryEntry entry) =>
        entry switch
        {
            CombatPreparationStartedHistoryEntry combatPreparationStartedHistoryEntry => combatPreparationStartedHistoryEntry.ToDto(),
            CombatStartedHistoryEntry combatStartedHistoryEntry => combatStartedHistoryEntry.ToDto(),
            CombatEntityJoinedHistoryEntry combatEntityJoinedHistoryEntry => combatEntityJoinedHistoryEntry.ToDto(),
            CombatEntityLeftHistoryEntry combatEntityLeftHistoryEntry => combatEntityLeftHistoryEntry.ToDto(),
            CombatEndedHistoryEntry combatEndedHistoryEntry => combatEndedHistoryEntry.ToDto(),
            _ => new CombatHistoryEntryDto { Tick = entry.Tick, Turn = entry.Turn }
        };
}
