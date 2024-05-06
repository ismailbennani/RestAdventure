using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Game.Apis.Common.Dtos.Jobs;

/// <summary>
///     Job instance
/// </summary>
public class JobInstanceDto
{
    /// <summary>
    ///     The job that is instantiated
    /// </summary>
    [Required]
    public required JobMinimalDto Job { get; init; }

    /// <summary>
    ///     The level of the job
    /// </summary>
    [Required]
    public required int Level { get; init; }

    /// <summary>
    ///     The experience of the job
    /// </summary>
    [Required]
    public required int Experience { get; init; }
}

static class JobInstanceMappingExtensions
{
    public static JobInstanceDto ToDto(this JobInstance jobInstance) =>
        new()
        {
            Job = jobInstance.Job.ToDto(),
            Level = jobInstance.Level,
            Experience = jobInstance.Experience
        };
}
