using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Game.Apis.Common.Dtos.Jobs;

/// <summary>
///     Job
/// </summary>
public class JobDto : JobMinimalDto
{
    /// <summary>
    ///     The description of the job
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     Is the job innate?
    /// </summary>
    [Required]
    public required bool Innate { get; init; }

    /// <summary>
    ///     The experience to reach each level of the job.
    /// </summary>
    [Required]
    public required IReadOnlyList<int> LevelCaps { get; init; }
}

static class JobMappingExtensions
{
    public static JobDto ToDto(this Job job) =>
        new()
        {
            Id = job.Id.Guid,
            Name = job.Name,
            Description = job.Description,
            Innate = job.Innate,
            LevelCaps = job.LevelCaps
        };
}
