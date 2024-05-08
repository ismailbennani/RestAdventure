using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Jobs;
using RestAdventure.Game.Apis.Common.Dtos.Jobs;
using RestAdventure.Game.Apis.Common.Dtos.StaticObjects;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     Harvest action
/// </summary>
public class HarvestActionDto : ActionDto
{
    /// <summary>
    ///     The harvest
    /// </summary>
    [Required]
    public required HarvestableEntityHarvestMinimalDto Harvest { get; init; }

    /// <summary>
    ///     The target of the harvest
    /// </summary>
    [Required]
    public required StaticObjectInstanceDto Target { get; init; }
}

static class HarvestActionMappingExtensions
{
    public static HarvestActionDto ToDto(this HarvestAction action) =>
        new()
        {
            Name = action.Name,
            Harvest = action.Harvest.ToDto(action.Job),
            Target = action.Target.ToDto()
        };
}
