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
    ///     The first team in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityInCombatDto> Team1 { get; init; }

    /// <summary>
    ///     The second team in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityInCombatDto> Team2 { get; init; }

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
            Team1 = combat.Team1.Entities.Select(e => e.ToEntityInCombatDto()).ToArray(),
            Team2 = combat.Team2.Entities.Select(e => e.ToEntityInCombatDto()).ToArray()
        };
}
