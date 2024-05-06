using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.History;

/// <summary>
///     Character learned job history entry
/// </summary>
public class CharacterLearnedJobHistoryEntryDto : CharacterHistoryEntryDto
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
}

static class CharacterLearnedJobHistoryEntryMappingExtensions
{
    public static CharacterLearnedJobHistoryEntryDto ToDto(this EntityLearnedJobHistoryEntry entry) =>
        new()
        {
            Tick = entry.Tick,
            JobId = entry.JobId.Guid,
            JobName = entry.JobName
        };
}
