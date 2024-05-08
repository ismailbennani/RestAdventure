using System.ComponentModel.DataAnnotations;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Jobs;

/// <summary>
///     Harvestable entity
/// </summary>
public class HarvestableEntityDto : EntityMinimalDto
{
    /// <summary>
    ///     The harvests available on the entity
    /// </summary>
    [Required]
    public required IReadOnlyCollection<HarvestableEntityHarvestDto> Harvests { get; init; }
}
