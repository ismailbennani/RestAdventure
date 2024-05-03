using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Maps;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Maps;

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
    public static MapAreaDto ToDto(this MapAreaDbo area) => new() { Id = area.Id, Name = area.Name };
}
