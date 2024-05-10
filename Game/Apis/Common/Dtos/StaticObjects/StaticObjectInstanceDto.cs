using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Entities.StaticObjects;

namespace RestAdventure.Game.Apis.Common.Dtos.StaticObjects;

/// <summary>
///     Static object instance
/// </summary>
public class StaticObjectInstanceDto
{
    /// <summary>
    ///     The unique ID of the entity
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The static object association with the entity
    /// </summary>
    [Required]
    public required StaticObjectDto StaticObject { get; init; }
}

static class StaticObjectInstanceMappingExtensions
{
    public static StaticObjectInstanceDto ToDto(this StaticObjectInstance staticObjectInstance) =>
        new()
        {
            Id = staticObjectInstance.Id.Guid,
            StaticObject = staticObjectInstance.Object.ToStaticObjectDto()
        };
}
