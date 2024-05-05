using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Game.Apis.Common.Dtos.Jobs;

/// <summary>
///     Job (minimal)
/// </summary>
public class JobMinimalDto
{
    /// <summary>
    ///     The unique ID of the job
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the job
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class JobMinimalMappingExtensions
{
    public static JobMinimalDto ToMinimalDto(this Job job) =>
        new()
        {
            Id = job.Id.Guid,
            Name = job.Name
        };
}
