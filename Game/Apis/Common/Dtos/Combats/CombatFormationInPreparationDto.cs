using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Combat formation
/// </summary>
public class CombatFormationInPreparationDto
{
    /// <summary>
    ///     The entities in the formation
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityMinimalDto> Entities { get; init; }

    /// <summary>
    ///     The options of the formation
    /// </summary>
    [Required]
    public required CombatFormationOptionsDto Options { get; init; }
}

static class CombatFormationInPreparationMappingExtensions
{
    public static CombatFormationInPreparationDto ToDto(this CombatFormationInPreparation formation) =>
        new()
        {
            Entities = formation.Entities.Select(e => e.ToMinimalDto()).ToArray(),
            Options = formation.Options.ToDto()
        };
}
