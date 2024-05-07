﻿using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Characters;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character ended interaction history entry
/// </summary>
public class CharacterEndedInteractionHistoryEntryDto : CharacterHistoryEntryDto
{
    /// <summary>
    ///     The name of the interaction that has been started
    /// </summary>
    [Required]
    public required string InteractionName { get; init; }

    /// <summary>
    ///     The entity that was the target of the interaction
    /// </summary>
    [Required]
    public required Guid TargetId { get; init; }

    /// <summary>
    ///     The name of the entity that was the target of the interaction
    /// </summary>
    [Required]
    public required string TargetName { get; init; }
}

static class CharacterEndedInteractionHistoryEntryMappingExtensions
{
    public static CharacterEndedInteractionHistoryEntryDto ToDto(this CharacterEndedInteractionHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            InteractionName = entry.InteractionName,
            TargetId = entry.TargetId.Guid,
            TargetName = entry.TargetName
        };
}
