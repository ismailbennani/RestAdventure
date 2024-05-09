using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Combat instance
/// </summary>
public class CombatInPreparationDto
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
    public required CombatFormationInPreparationDto Attackers { get; init; }

    /// <summary>
    ///     The defenders in the combat instance
    /// </summary>
    [Required]
    public required CombatFormationInPreparationDto Defenders { get; init; }

    /// <summary>
    ///     Can the character join the combat
    /// </summary>
    [Required]
    public required bool CanJoin { get; init; }

    /// <summary>
    ///     Why cannot the character join the combat
    /// </summary>
    [Required]
    public string? WhyCannotJoin { get; init; }
}

static class CombatInPreparationMappingExtensions
{
    public static CombatInPreparationDto ToDto(this CombatInPreparation combat, Maybe canJoin) =>
        new()
        {
            Id = combat.Id.Guid,
            Attackers = combat.Attackers.ToDto(),
            Defenders = combat.Defenders.ToDto(),
            CanJoin = canJoin.Success,
            WhyCannotJoin = canJoin.WhyNot
        };
}
