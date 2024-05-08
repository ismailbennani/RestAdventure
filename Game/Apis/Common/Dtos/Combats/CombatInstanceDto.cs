using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Combat instance
/// </summary>
public class CombatInstanceDto
{
    /// <summary>
    ///     The unique ID of the combat instance
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The attackers in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityInCombatDto> Attackers { get; init; }

    /// <summary>
    ///     The defenders in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityInCombatDto> Defenders { get; init; }

    /// <summary>
    ///     The current turn of the combat instance
    /// </summary>
    [Required]
    public int Turn { get; init; }
}

static class CombatInstanceMappingExtensions
{
    public static CombatInstanceDto ToDto(this CombatInstance combat) =>
        new()
        {
            Id = combat.Id.Guid,
            Turn = combat.Turn,
            Attackers = combat.Attackers.Entities.Select(e => e.ToEntityInCombatDto()).ToArray(),
            Defenders = combat.Defenders.Entities.Select(e => e.ToEntityInCombatDto()).ToArray()
        };
}
