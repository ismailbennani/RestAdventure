using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Game.Apis.Common.Dtos.Combats;
using RestAdventure.Game.Apis.Common.Dtos.Monsters;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     PVE combat action
/// </summary>
public class JoinPveCombatActionDto : ActionDto
{
    /// <summary>
    ///     The group of monsters
    /// </summary>
    [Required]
    public required MonsterGroupMinimalDto MonsterGroup { get; init; }

    /// <summary>
    ///     The combat to join
    /// </summary>
    [Required]
    public required CombatInPreparationDto Combat { get; init; }
}

static class JoinPveCombatActionMappingExtensions
{
    public static JoinPveCombatActionDto ToDto(this JoinAndPlayPveCombatAction action) =>
        new()
        {
            Name = action.Name,
            MonsterGroup = action.MonsterGroup.ToMonsterGroupMinimalDto(),
            Combat = action.Combat.ToCombatInPreparation()
        };
}
