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
    ///     The locations connected to this one
    /// </summary>
    [Required]
    public required IReadOnlyCollection<MapLocationMinimalDto> ConnectedLocations { get; init; }
}

static class MapLocationMappingExtensions
{
    public static MapLocationDto ToDto(this MapLocation location, GameContent content) =>
        new()
        {
            Id = location.Id.Guid, Area = location.Area.ToDto(), PositionX = location.PositionX, PositionY = location.PositionY,
            ConnectedLocations = content.Maps.GetConnectedLocations(location).Select(l => l.ToMinimalDto()).ToArray()
        };
}
