using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Game.Apis.Common.Dtos.Maps;

/// <summary>
///     Map location minimal information
/// </summary>
public class MapLocationMinimalDto
{
    /// <summary>
    ///     The unique ID of the location
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The area associated with the location
    /// </summary>
    [Required]
    public required MapAreaDto Area { get; init; }

    /// <summary>
    ///     The X coordinate associated with the location
    /// </summary>
    [Required]
    public required int PositionX { get; init; }

    /// <summary>
    ///     The Y coordinate associated with the location
    /// </summary>
    [Required]
    public required int PositionY { get; init; }
}

static class MapLocationMinimalMappingExtensions
{
    public static MapLocationMinimalDto ToMinimalDto(this MapLocation location) =>
        new() { Id = location.Id.Guid, Area = location.Area.ToDto(), PositionX = location.PositionX, PositionY = location.PositionY };
}
