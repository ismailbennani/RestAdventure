using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Maps;

namespace RestAdventure.Game.Apis.Common.Dtos.Entities;

/// <summary>
///     Entity
/// </summary>
public class EntityDto : EntityMinimalDto
{
    /// <summary>
    ///     The location of the entity
    /// </summary>
    [Required]
    public required LocationMinimalDto Location { get; init; }
}

static class EntityMappingExtensions
{
    public static EntityDto ToDto(this GameEntitySnapshot entity) =>
        new()
        {
            Id = entity.Id.Guid,
            Name = entity.Name,
            Location = entity.Location.ToMinimalDto()
        };
}
