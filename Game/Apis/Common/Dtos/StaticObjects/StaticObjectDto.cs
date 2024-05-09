using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Entities.StaticObjects;

namespace RestAdventure.Game.Apis.Common.Dtos.StaticObjects;

/// <summary>
///     Static object
/// </summary>
public class StaticObjectDto
{
    /// <summary>
    ///     The unique ID of the static object
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the static object
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     The description of the static object
    /// </summary>
    public string? Description { get; init; }
}

static class StaticObjectMappingExtensions
{
    public static StaticObjectDto ToDto(this StaticObject staticObject) =>
        new()
        {
            Id = staticObject.Id.Guid,
            Name = staticObject.Name,
            Description = staticObject.Description
        };
}
