using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Utils;

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
    ///     The progression of the job
    /// </summary>
    [Required]
    public required ProgressionBarMinimalDto Progression { get; init; }
}

static class JobInstanceMappingExtensions
{
    public static JobInstanceDto ToDto(this JobInstance jobInstance) =>
        new()
        {
            Job = jobInstance.Job.ToDto(),
            Progression = jobInstance.Progression.ToMinimalDto()
        };

    public static JobInstanceDto ToDto(this JobInstanceSnapshot jobInstance) =>
        new()
        {
            Job = jobInstance.Job.ToDto(),
            Progression = jobInstance.Progression.ToMinimalDto()
        };
}
