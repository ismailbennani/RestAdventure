using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RestAdventure.Core.History.Actions;
using RestAdventure.Core.History.Characters;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Characters;

/// <summary>
///     Character history entry
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type", IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(CharacterCreatedHistoryEntryDto), "created")]
[JsonDerivedType(typeof(CharacterDeletedHistoryEntryDto), "deleted")]
[JsonDerivedType(typeof(CharacterMovedToLocationHistoryEntryDto), "moved")]
[JsonDerivedType(typeof(CharacterInventoryChangedHistoryEntryDto), "inventory-changed")]
[JsonDerivedType(typeof(ActionStartedHistoryEntryDto), "interaction-started")]
[JsonDerivedType(typeof(ActionEndedHistoryEntryDto), "interaction-ended")]
[JsonDerivedType(typeof(CharacterLearnedJobHistoryEntryDto), "job-learned")]
[JsonDerivedType(typeof(CharacterJobGainedExperienceHistoryEntryDto), "job-gained-experience")]
[JsonDerivedType(typeof(CharacterJobLeveledUpHistoryEntryDto), "job-leveled-up")]
[JsonDerivedType(typeof(CharacterStartedCombatPreparationHistoryEntryDto), "combat-preparation-started")]
[JsonDerivedType(typeof(CharacterCombatStartedHistoryEntryDto), "combat-started")]
[JsonDerivedType(typeof(CharacterCombatEndedHistoryEntryDto), "combat-ended")]
public class CharacterHistoryEntryDto
{
    /// <summary>
    ///     The tick at which the event happened
    /// </summary>
    [Required]
    public required long Tick { get; init; }
}

static class CharacterHistoryEntryMappingExtensions
{
    public static CharacterHistoryEntryDto ToDto(this EntityHistoryEntry entry) =>
        entry switch
        {
            EntityCreatedHistoryEntry entityCreatedHistoryEntry => entityCreatedHistoryEntry.ToDto(),
            EntityDeletedHistoryEntry entityDeletedHistoryEntry => entityDeletedHistoryEntry.ToDto(),
            ActionStartedHistoryEntry characterStartedInteractionHistoryEntry => characterStartedInteractionHistoryEntry.ToDto(),
            ActionEndedHistoryEntry characterEndedInteractionHistoryEntry => characterEndedInteractionHistoryEntry.ToDto(),
            EntityInventoryChangedHistoryEntry entityInventoryChangedHistoryEntry => entityInventoryChangedHistoryEntry.ToDto(),
            EntityMovedToLocationHistoryEntry entityMovedToLocationHistoryEntry => entityMovedToLocationHistoryEntry.ToDto(),
            EntityLearnedJobHistoryEntry entityLearnedJobHistoryEntry => entityLearnedJobHistoryEntry.ToDto(),
            EntityJobGainedExperienceHistoryEntry entityJobGainedExperienceHistoryEntry => entityJobGainedExperienceHistoryEntry.ToDto(),
            EntityJobLeveledUpHistoryEntry entityJobLeveledUpHistoryEntry => entityJobLeveledUpHistoryEntry.ToDto(),
            CharacterStartedCombatPreparationHistoryEntry characterStartedCombatPreparationHistoryEntry => characterStartedCombatPreparationHistoryEntry.ToDto(),
            CharacterCombatStartedHistoryEntry characterCombatStartedHistoryEntry => characterCombatStartedHistoryEntry.ToDto(),
            CharacterCombatEndedHistoryEntry characterCombatEndedHistoryEntry => characterCombatEndedHistoryEntry.ToDto(),
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };
}
