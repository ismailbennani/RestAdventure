﻿using System.ComponentModel.DataAnnotations;
using RestAdventure.Core;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Game.Apis.Common.Dtos.Maps;

/// <summary>
///     Map location
/// </summary>
public class LocationDto : LocationMinimalDto
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
    public required IReadOnlyCollection<LocationMinimalDto> ConnectedLocations { get; init; }
}

static class LocationMappingExtensions
{
    public static LocationDto ToDiscoveredLocationDto(this Location location, GameContent content) =>
        new()
        {
            Id = location.Id.Guid,
            Area = location.Area.ToDto(),
            PositionX = location.PositionX,
            PositionY = location.PositionY,
            Discovered = true,
            ConnectedLocations = content.Maps.Locations.ConnectedTo(location).Select(l => l.ToMinimalDto()).ToArray()
        };

    public static LocationDto ToUndiscoveredLocationDto(this Location location) =>
        new()
        {
            Id = location.Id.Guid,
            Area = location.Area.ToDto(),
            PositionX = location.PositionX,
            PositionY = location.PositionY,
            Discovered = false,
            ConnectedLocations = Array.Empty<LocationMinimalDto>()
        };
}
