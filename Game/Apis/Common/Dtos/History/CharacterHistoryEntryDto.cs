using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RestAdventure.Core.History.Characters;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character history entry
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type", IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(CharacterCreatedHistoryEntryDto), "created")]
[JsonDerivedType(typeof(CharacterDeletedHistoryEntryDto), "deleted")]
[JsonDerivedType(typeof(CharacterMovedToLocationHistoryEntryDto), "moved")]
[JsonDerivedType(typeof(CharacterInventoryChangedHistoryEntryDto), "inventory-changed")]
[JsonDerivedType(typeof(CharacterPerformedActionHistoryEntryDto), "action-performed")]
[JsonDerivedType(typeof(CharacterStartedInteractionHistoryEntryDto), "interaction-started")]
[JsonDerivedType(typeof(CharacterEndedInteractionHistoryEntryDto), "interaction-ended")]
[JsonDerivedType(typeof(CharacterLearnedJobHistoryEntryDto), "job-learned")]
[JsonDerivedType(typeof(CharacterJobGainedExperienceHistoryEntryDto), "job-gained-experience")]
[JsonDerivedType(typeof(CharacterJobLeveledUpHistoryEntryDto), "job-leveled-up")]
[JsonDerivedType(typeof(CharacterAttackedHistoryEntryDto), "attacked")]
[JsonDerivedType(typeof(CharacterReceivedAttackHistoryEntryDto), "received-attack")]
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
            CharacterPerformedActionHistoryEntry characterPerformedActionHistoryEntry => characterPerformedActionHistoryEntry.ToDto(),
            CharacterStartedInteractionHistoryEntry characterStartedInteractionHistoryEntry => characterStartedInteractionHistoryEntry.ToDto(),
            CharacterEndedInteractionHistoryEntry characterEndedInteractionHistoryEntry => characterEndedInteractionHistoryEntry.ToDto(),
            EntityInventoryChangedHistoryEntry entityInventoryChangedHistoryEntry => entityInventoryChangedHistoryEntry.ToDto(),
            EntityMovedToLocationHistoryEntry entityMovedToLocationHistoryEntry => entityMovedToLocationHistoryEntry.ToDto(),
            EntityLearnedJobHistoryEntry entityLearnedJobHistoryEntry => entityLearnedJobHistoryEntry.ToDto(),
            EntityJobGainedExperienceHistoryEntry entityJobGainedExperienceHistoryEntry => entityJobGainedExperienceHistoryEntry.ToDto(),
            EntityJobLeveledUpHistoryEntry entityJobLeveledUpHistoryEntry => entityJobLeveledUpHistoryEntry.ToDto(),
            CharacterAttackedHistoryEntry characterAttackedHistoryEntry => characterAttackedHistoryEntry.ToDto(),
            CharacterReceivedAttackHistoryEntry characterReceivedAttackHistoryEntry => characterReceivedAttackHistoryEntry.ToDto(),
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };
}
