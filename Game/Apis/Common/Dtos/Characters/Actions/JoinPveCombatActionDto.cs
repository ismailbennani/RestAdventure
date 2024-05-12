using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat.Pve;

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
    public required Guid MonsterGroupId { get; init; }

    /// <summary>
    ///     The combat to join
    /// </summary>
    [Required]
    public required Guid CombatId { get; init; }
}

static class JoinPveCombatActionMappingExtensions
{
    public static JoinPveCombatActionDto ToDto(this JoinAndPlayPveCombatAction action) =>
        new()
        {
            Name = action.Name,
            MonsterGroupId = action.MonsterGroupId.Guid,
            CombatId = action.CombatId.Guid
        };
}
