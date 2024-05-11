using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Combat instance
/// </summary>
public class OngoingCombatDto : CombatDto
{
    /// <summary>
    ///     The current turn of the combat instance
    /// </summary>
    [Required]
    public int Turn { get; init; }

    /// <summary>
    ///     The attackers in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<CombatEntityDto> Attackers { get; init; }

    /// <summary>
    ///     The defenders in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<CombatEntityDto> Defenders { get; init; }
}

static class CombatInstanceMappingExtensions
{
    public static OngoingCombatDto ToOngoingCombatDto(this CombatInstance combat) =>
        new()
        {
            Id = combat.Id.Guid,
            Turn = combat.Turn,
            Attackers = combat.AttackerCombatEntities.Select(e => e.ToDto()).ToArray(),
            Defenders = combat.DefenderCombatEntities.Select(e => e.ToDto()).ToArray()
        };
}
