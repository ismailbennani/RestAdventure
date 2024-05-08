using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;
using RestAdventure.Game.Apis.Common.Dtos.Entities;
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
    public required IReadOnlyList<EntityMinimalDto> Attackers { get; init; }

    /// <summary>
    ///     The defenders in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityMinimalDto> Defenders { get; init; }

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
            Attackers = combat.Attackers.Entities.Select(e => e.ToMinimalDto()).ToArray(),
            Defenders = combat.Defenders.Entities.Select(e => e.ToMinimalDto()).ToArray(),
            CanJoin = canJoin.Success,
            WhyCannotJoin = canJoin.WhyNot
        };
}
