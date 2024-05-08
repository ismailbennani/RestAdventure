using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.Common.Dtos.Jobs;

/// <summary>
///     Harvestable entity harvest
/// </summary>
public class HarvestableEntityHarvestDto : HarvestableEntityHarvestMinimalDto
{
    /// <summary>
    ///     Can the harvest be performed
    /// </summary>
    [Required]
    public required bool CanHarvest { get; init; }

    /// <summary>
    ///     Why cannot the harvest be performed
    /// </summary>
    public string? WhyCannotHarvest { get; init; }
}
