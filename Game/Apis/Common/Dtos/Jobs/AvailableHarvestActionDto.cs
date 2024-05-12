using System.ComponentModel.DataAnnotations;
using RestAdventure.Game.Apis.Common.Dtos.Items;

namespace RestAdventure.Game.Apis.Common.Dtos.Jobs;

/// <summary>
///     Available harvest action
/// </summary>
public class AvailableHarvestActionDto
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
    ///     If set, the tool required for the harvest
    /// </summary>
    public required ItemCategoryDto? Tool { get; init; }

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
