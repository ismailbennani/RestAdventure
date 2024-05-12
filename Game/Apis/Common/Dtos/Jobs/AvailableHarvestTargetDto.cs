using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.Common.Dtos.Jobs;

/// <summary>
///     Available harvest target
/// </summary>
public class AvailableHarvestTargetDto
{
    /// <summary>
    ///     The object of the harvest
    /// </summary>
    [Required]
    public required Guid ObjectId { get; set; }

    /// <summary>
    ///     The instance of the object that is the target of the harvest
    /// </summary>
    [Required]
    public required Guid ObjectInstanceId { get; init; }

    /// <summary>
    ///     The available harvest actions
    /// </summary>
    [Required]
    public required IReadOnlyCollection<AvailableHarvestActionDto> Actions { get; init; }
}
