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
    ///     The first team in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityMinimalDto> Team1 { get; init; }

    /// <summary>
    ///     The second team in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityMinimalDto> Team2 { get; init; }
}

static class CombatInPreparationMappingExtensions
{
    public static CombatInPreparationDto ToDto(this CombatInPreparation combat) =>
        new()
        {
            Id = combat.Id.Guid,
            Team1 = combat.Team1.Entities.Select(e => e.ToMinimalDto()).ToArray(),
            Team2 = combat.Team2.Entities.Select(e => e.ToMinimalDto()).ToArray()
        };
}
