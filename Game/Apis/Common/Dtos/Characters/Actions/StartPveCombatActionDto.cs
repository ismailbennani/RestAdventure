using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     PVE combat action
/// </summary>
public class StartPveCombatActionDto : ActionDto
{
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

static class StartPveCombatActionMappingExtensions
{
    public static StartPveCombatActionDto ToDto(this StartPveCombatAction action) =>
        new()
        {
            Name = action.Name,
            Attackers = action.Attackers.Select(e => e.ToMinimalDto()).ToArray(),
            Defenders = action.Defenders.Select(e => e.ToMinimalDto()).ToArray()
        };
}
