using System.ComponentModel.DataAnnotations;
using RestAdventure.Core;
using RestAdventure.Core.Maps;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Maps;

/// <summary>
///     Map location
/// </summary>
public class MapLocationDto : MapLocationMinimalDto
{
    /// <summary>
    ///     Has this location been discovered by the player.
    ///     If false, the connected locations will be hidden.
    /// </summary>
    [Required]
    public required bool Discovered { get; init; }

    /// <summary>
    ///     The locations connected to this one
    /// </summary>
    [Required]
    public required IReadOnlyCollection<MapLocationMinimalDto> ConnectedLocations { get; init; }
}

static class MapLocationMappingExtensions
{
    public static MapLocationDto ToDiscoveredLocationDto(this MapLocation location, GameContent content) =>
        new()
        {
            Id = location.Id.Guid,
            Area = location.Area.ToDto(),
            PositionX = location.PositionX,
            PositionY = location.PositionY,
            Discovered = true,
            ConnectedLocations = content.Maps.GetConnectedLocations(location).Select(l => l.ToMinimalDto()).ToArray()
        };

    public static MapLocationDto ToUndiscoveredLocationDto(this MapLocation location) =>
        new()
        {
            Id = location.Id.Guid,
            Area = location.Area.ToDto(),
            PositionX = location.PositionX,
            PositionY = location.PositionY,
            Discovered = false,
            ConnectedLocations = Array.Empty<MapLocationMinimalDto>()
        };
}
