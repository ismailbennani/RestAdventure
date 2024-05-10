using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Jobs;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.StaticObjects;

namespace RestAdventure.Game.Apis.Common.Dtos.Jobs;

/// <summary>
///     Harvestable entity harvest (minimal
/// </summary>
public class HarvestableEntityHarvestMinimalDto
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
    ///     The targets compatible with the harvest
    /// </summary>
    [Required]
    public required IReadOnlyCollection<StaticObjectDto> Targets { get; init; }

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

static class HarvestableEntityHarvestMinimalMappingExtensions
{
    public static HarvestableEntityHarvestMinimalDto ToMinimalDto(this JobHarvest harvest, Job job) =>
        new()
        {
            Job = job.ToMinimalDto(),
            Name = harvest.Name,
            Targets = harvest.Targets.Select(t => t.ToStaticObjectDto()).ToArray(),
            ExpectedHarvest = harvest.Items.Select(e => e.ToDto()).ToArray(),
            ExpectedExperience = harvest.Experience
        };
}
