using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Game.Apis.Common.Dtos.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Monsters;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     PVE combat action
/// </summary>
public class PveCombatActionDto : ActionDto
{
    /// <summary>
    ///     The unique ID of the combat
    /// </summary>
    [Required]
    public required Guid CombatId { get; init; }

    /// <summary>
    ///     The attackers in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityMinimalDto> Attackers { get; init; }

    /// <summary>
    ///     The group of monsters
    /// </summary>
    [Required]
    public required MonsterGroupMinimalDto MonsterGroup { get; init; }
}

static class PveCombatActionMappingExtensions
{
    public static PveCombatActionDto ToDto(this PveCombatAction action) =>
        new()
        {
            Name = action.Name,
            CombatId = action.CombatInPreparation?.Id.Guid ?? action.Combat?.Id.Guid ?? default,
            Attackers = action.Attackers.Select(e => e.ToMinimalDto()).ToArray(),
            MonsterGroup = action.MonsterGroup.ToMonsterGroupMinimalDto()
        };
}
