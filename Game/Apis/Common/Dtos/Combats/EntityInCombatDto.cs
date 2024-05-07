using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Entity in combat
/// </summary>
public class EntityInCombatDto : EntityMinimalDto
{
    /// <summary>
    ///     The level of the character
    /// </summary>
    [Required]
    public required int Level { get; init; }

    /// <summary>
    ///     The combat statistics of the character
    /// </summary>
    [Required]
    public required EntityCombatStatisticsDto Combat { get; init; }
}

static class EntityInCombatMappingExtensions
{
    public static EntityInCombatDto ToEntityInCombatDto(this IGameEntityWithCombatStatistics entity) =>
        new()
        {
            Id = entity.Id.Guid,
            Name = entity.Name,
            Level = entity.Level,
            Combat = entity.Combat.ToDto()
        };
}
