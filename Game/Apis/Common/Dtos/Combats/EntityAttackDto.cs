using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat;

namespace RestAdventure.Game.Apis.Common.Dtos.Combats;

/// <summary>
///     Entity attack
/// </summary>
public class EntityAttackDto
{
    /// <summary>
    ///     The damage of the attack
    /// </summary>
    [Required]
    public required int Damage { get; init; }
}

static class EntityAttackMappingExtensions
{
    public static EntityAttackDto ToDto(this CombatEntityAttack attack) =>
        new()
        {
            Damage = attack.Damage
        };
}
