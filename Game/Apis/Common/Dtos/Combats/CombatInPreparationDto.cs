using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Combat in preparation
/// </summary>
public class CombatInPreparationDto : CombatDto
{
    /// <summary>
    ///     The attackers in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityMinimalDto> Attackers { get; init; }

    /// <summary>
    ///     The options of the attacker side
    /// </summary>
    [Required]
    public required CombatFormationOptionsDto AttackersOptions { get; init; }

    /// <summary>
    ///     The defenders in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityMinimalDto> Defenders { get; init; }

    /// <summary>
    ///     The options of the defender side
    /// </summary>
    [Required]
    public required CombatFormationOptionsDto DefendersOptions { get; init; }
}

static class CombatInPreparationMappingExtensions
{
    public static CombatInPreparationDto ToCombatInPreparation(this CombatInstance instance) =>
        new()
        {
            Id = instance.Id.Guid,
            Attackers = instance.Attackers.Entities.Select(e => e.ToMinimalDto()).ToArray(),
            AttackersOptions = instance.Attackers.Options.ToDto(),
            Defenders = instance.Defenders.Entities.Select(e => e.ToMinimalDto()).ToArray(),
            DefendersOptions = instance.Defenders.Options.ToDto()
        };
}
