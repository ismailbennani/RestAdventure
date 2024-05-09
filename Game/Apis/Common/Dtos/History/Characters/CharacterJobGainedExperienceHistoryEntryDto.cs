using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History.Characters;

/// <summary>
///     Character learned job history entry
/// </summary>
public class CharacterJobGainedExperienceHistoryEntryDto : CharacterHistoryEntryDto
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
    public required int OldExperience { get; init; }

    /// <summary>
    ///     The new level of the job
    /// </summary>
    [Required]
    public required int NewExperience { get; init; }
}

static class CharacterJobGainedExperienceHistoryEntryMappingExtensions
{
    public static CharacterJobGainedExperienceHistoryEntryDto ToDto(this EntityJobGainedExperienceHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            JobId = entry.JobId.Guid,
            JobName = entry.JobName,
            OldExperience = entry.OldExperience,
            NewExperience = entry.NewExperience
        };
}
