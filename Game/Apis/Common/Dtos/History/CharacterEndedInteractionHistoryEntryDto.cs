using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Characters;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character ended interaction history entry
/// </summary>
public class CharacterEndedInteractionHistoryEntryDto : CharacterHistoryEntryDto
{
    /// <summary>
    ///     The unique ID of the interaction that has been started
    /// </summary>
    [Required]
    public required Guid InteractionId { get; init; }

    /// <summary>
    ///     The name of the interaction that has been started
    /// </summary>
    [Required]
    public required string InteractionName { get; init; }

    /// <summary>
    ///     The entity that was the subject of the interaction
    /// </summary>
    [Required]
    public required Guid SubjectId { get; init; }

    /// <summary>
    ///     The name of the entity that was the subject of the interaction
    /// </summary>
    [Required]
    public required string SubjectName { get; init; }
}

static class CharacterEndedInteractionHistoryEntryMappingExtensions
{
    public static CharacterEndedInteractionHistoryEntryDto ToDto(this CharacterEndedInteractionHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            InteractionId = entry.InteractionId.Guid,
            InteractionName = entry.InteractionName,
            SubjectId = entry.SubjectId.Guid,
            SubjectName = entry.SubjectName
        };
}
