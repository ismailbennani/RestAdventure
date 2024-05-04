using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Maps;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Maps;

public class MapLocationDto : MapLocationMinimalDto
{
    /// <summary>
    ///     The locations connected to this one
    /// </summary>
    [Required]
    public required IReadOnlyCollection<MapLocationMinimalDto> ConnectedLocations { get; init; }
}

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

static class MapLocationMappingExtensions
{
    public static MapLocationMinimalDto ToMinimalDto(this MapLocation location) =>
        new() { Id = location.Id.Guid, Area = location.Area.ToDto(), PositionX = location.PositionX, PositionY = location.PositionY };

    public static MapLocationDto ToDto(this MapLocation location) =>
        new()
        {
            Id = location.Id.Guid, Area = location.Area.ToDto(), PositionX = location.PositionX, PositionY = location.PositionY,
            ConnectedLocations = location.ConnectedLocations.Select(l => l.ToMinimalDto()).ToArray()
        };
}
