using RestAdventure.Core.Combat;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters;

/// <summary>
///     Entity combat statistics
/// </summary>
public class EntityCombatStatisticsDto
{
    /// <summary>
    ///     The health of the entity
    /// </summary>
    public required int Health { get; init; }

    /// <summary>
    ///     The max health of the entity
    /// </summary>
    public required int MaxHealth { get; init; }

    /// <summary>
    ///     The speed of the entity
    /// </summary>
    public required int Speed { get; init; }

    /// <summary>
    ///     The attack of the entity
    /// </summary>
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
