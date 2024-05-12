using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat.Pve;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     PVE combat action
/// </summary>
public class StartPveCombatActionDto : ActionDto
{
    /// <summary>
    ///     The group of monsters
    /// </summary>
    [Required]
    public required Guid MonsterGroupId { get; init; }
}

static class StartPveCombatActionMappingExtensions
{
    public static StartPveCombatActionDto ToDto(this StartAndPlayPveCombatAction action) =>
        new()
        {
            Name = action.Name,
            MonsterGroupId = action.MonsterGroupId.Guid
        };
}
