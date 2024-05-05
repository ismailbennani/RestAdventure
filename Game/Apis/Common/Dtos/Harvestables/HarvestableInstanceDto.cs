using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Maps.Harvestables;

namespace RestAdventure.Game.Apis.Common.Dtos.Harvestables;

/// <summary>
///     Harvestable entity
/// </summary>
public class HarvestableInstanceDto
{
    /// <summary>
    ///     The unique ID of the entity
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the entity
    /// </summary>
    [Required]
    public required HarvestableDto Harvestable { get; init; }

    /// <summary>
    ///     Can this harvestable entity be harvested
    /// </summary>
    [Required]
    public required bool CanHarvest { get; init; }
}

static class HarvestableInstanceMappingExtensions
{
    public static HarvestableInstanceDto ToDto(this HarvestableInstance harvestable, bool canHarvest) =>
        new()
        {
            Id = harvestable.Id.Guid,
            Harvestable = harvestable.Harvestable.ToDto(),
            CanHarvest = canHarvest
        };
}
