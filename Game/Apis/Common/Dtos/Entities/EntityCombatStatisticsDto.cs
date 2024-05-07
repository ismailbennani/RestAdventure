using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;

namespace RestAdventure.Game.Apis.Common.Dtos.Entities;

/// <summary>
///     Entity combat statistics
/// </summary>
public class EntityCombatStatisticsDto
{
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

    /// <summary>
    ///     The speed of the entity
    /// </summary>
    [Required]
    public required int Speed { get; init; }

    /// <summary>
    ///     The attack of the entity
    /// </summary>
    [Required]
    public required int Attack { get; init; }
}

static class EntityCombatStatisticsMappingExtensions
{
    public static EntityCombatStatisticsDto ToDto(this EntityCombatStatistics combat) =>
        new()
        {
            Health = combat.Health,
            MaxHealth = combat.MaxHealth,
            Speed = combat.Speed,
            Attack = combat.Attack
        };
}
