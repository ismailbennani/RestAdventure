using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character learned job history entry
/// </summary>
public class CharacterJobLeveledUpHistoryEntryDto : CharacterHistoryEntryDto
{
    /// <summary>
    ///     The unique ID of the job
    /// </summary>
    [Required]
    public required Guid JobId { get; init; }

    /// <summary>
    ///     The name of the job
    /// </summary>
    [Required]
    public required string JobName { get; init; }

    /// <summary>
    ///     The old level of the job
    /// </summary>
    [Required]
    public required int OldLevel { get; init; }

    /// <summary>
    ///     The new level of the job
    /// </summary>
    [Required]
    public required int NewLevel { get; init; }
}

static class CharacterJobLeveledUpHistoryEntryMappingExtensions
{
    public static CharacterJobLeveledUpHistoryEntryDto ToDto(this EntityJobLeveledUpHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            JobId = entry.JobId.Guid,
            JobName = entry.JobName,
            OldLevel = entry.OldLevel,
            NewLevel = entry.NewLevel
        };
}
