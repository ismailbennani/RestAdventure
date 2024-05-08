using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     PVE combat action
/// </summary>
public class JoinPveCombatActionDto : ActionDto
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
    ///     The defenders in the combat instance
    /// </summary>
    [Required]
    public required IReadOnlyList<EntityMinimalDto> Defenders { get; init; }
}

static class JoinPveCombatActionMappingExtensions
{
    public static JoinPveCombatActionDto ToDto(this JoinPveCombatAction action) =>
        new()
        {
            Name = action.Name,
            CombatId = action.Combat.Id.Guid,
            Attackers = action.Combat.Attackers.Entities.Select(e => e.ToMinimalDto()).ToArray(),
            Defenders = action.Combat.Defenders.Entities.Select(e => e.ToMinimalDto()).ToArray()
        };
}
