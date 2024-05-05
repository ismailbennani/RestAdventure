using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Maps.Harvestables;

namespace RestAdventure.Game.Apis.Common.Dtos.Harvestables;

/// <summary>
///     Harvestable
/// </summary>
public class HarvestableDto
{
    /// <summary>
    ///     The unique ID of the harvestable
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the harvestable
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     The description of the harvestable
    /// </summary>
    public string? Description { get; init; }
}

static class HarvestableMappingExtensions
{
    public static HarvestableDto ToDto(this MapHarvestable harvestable) =>
        new()
        {
            Id = harvestable.Id.Guid,
            Name = harvestable.Name,
            Description = harvestable.Description
        };
}
