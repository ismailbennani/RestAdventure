using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

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
}

static class CombatInPreparationMappingExtensions
{
    public static CombatInPreparationDto ToDto(this CombatInPreparation combat) =>
        new()
        {
            Id = combat.Id.Guid,
            Attackers = combat.Attackers.Entities.Select(e => e.ToMinimalDto()).ToArray(),
            Defenders = combat.Defenders.Entities.Select(e => e.ToMinimalDto()).ToArray()
        };
}
