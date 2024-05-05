using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Maps.Areas;

namespace RestAdventure.Game.Apis.Common.Dtos.Maps;

/// <summary>
///     Map area
/// </summary>
public class MapAreaDto
{
    /// <summary>
    ///     The unique ID of the area
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the area
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class MapAreaMappingExtensions
{
    public static MapAreaDto ToDto(this MapArea area) => new() { Id = area.Id.Guid, Name = area.Name };
}
