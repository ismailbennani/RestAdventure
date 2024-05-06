﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RestAdventure.Core.History.Characters;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character history entry
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type", IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(CharacterMovedToLocationHistoryEntryDto), "moved")]
[JsonDerivedType(typeof(CharacterInventoryChangedHistoryEntryDto), "inventory-changed")]
[JsonDerivedType(typeof(CharacterStartedInteractionHistoryEntryDto), "interaction-started")]
[JsonDerivedType(typeof(CharacterEndedInteractionHistoryEntryDto), "interaction-ended")]
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
            CharacterEndedInteractionHistoryEntry characterEndedInteractionHistoryEntry => characterEndedInteractionHistoryEntry.ToDto(),
            CharacterStartedInteractionHistoryEntry characterStartedInteractionHistoryEntry => characterStartedInteractionHistoryEntry.ToDto(),
            EntityInventoryChangedHistoryEntry entityInventoryChangedHistoryEntry => entityInventoryChangedHistoryEntry.ToDto(),
            EntityMovedToLocationHistoryEntry entityMovedToLocationHistoryEntry => entityMovedToLocationHistoryEntry.ToDto(),
            _ => throw new ArgumentOutOfRangeException(nameof(entry))
        };
}