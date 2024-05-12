using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Serialization.Combats;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Entity in combat
/// </summary>
public class CombatEntityDto : EntityMinimalDto
{
    /// <summary>
    ///     The level of the character
    /// </summary>
    [Required]
    public required int Level { get; init; }

    /// <summary>
    ///     The health of the entity
    /// </summary>
    [Required]
    public required int Health { get; init; }

    /// <summary>
    ///     The max health of the entity
    /// </summary>
    [Required]
    public required int MaxHealth { get; init; }
}

static class EntityInCombatMappingExtensions
{
    public static CombatEntityDto ToDto(this CombatEntitySnapshot entity) =>
        new()
        {
            Id = entity.Id.Guid,
            Name = entity.Name,
            Level = entity.Level,
            Health = entity.Health,
            MaxHealth = entity.MaxHealth
        };
}
