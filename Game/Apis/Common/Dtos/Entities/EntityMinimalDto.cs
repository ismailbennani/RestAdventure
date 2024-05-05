using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Entities;

/// <summary>
///     Entity (minimal)
/// </summary>
public class EntityMinimalDto
{
    /// <summary>
    ///     The unique ID of the entity
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the entity
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class EntityMinimalMappingExtensions
{
    public static EntityMinimalDto ToMinimalDto(this IGameEntity entity) => new() { Id = entity.Id.Guid, Name = entity.Name };
}
