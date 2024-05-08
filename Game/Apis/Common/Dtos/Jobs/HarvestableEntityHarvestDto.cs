using System.ComponentModel.DataAnnotations;
using RestAdventure.Game.Apis.Common.Dtos.Items;

namespace RestAdventure.Game.Apis.Common.Dtos.Jobs;

/// <summary>
///     Job harvest
/// </summary>
public class HarvestableEntityHarvestDto
{
    /// <summary>
    ///     The job providing the harvest
    /// </summary>
    [Required]
    public required JobMinimalDto Job { get; init; }

    /// <summary>
    ///     The name of the harvest
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     Can the harvest be performed
    /// </summary>
    [Required]
    public required bool CanHarvest { get; init; }

    /// <summary>
    ///     The expected result of the harvest
    /// </summary>
    [Required]
    public required IReadOnlyCollection<ItemStackDto> ExpectedHarvest { get; init; }

    /// <summary>
    ///     The expected experience gain from the harvest
    /// </summary>
    [Required]
    public required int ExpectedExperience { get; init; }
}
