﻿using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Maps;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Maps;

public class MapLocationDto
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
    public static MapLocationDto ToDto(this MapLocationDbo location) =>
        new() { Id = location.Id, Area = location.Area.ToDto(), PositionX = location.PositionX, PositionY = location.PositionY };
}