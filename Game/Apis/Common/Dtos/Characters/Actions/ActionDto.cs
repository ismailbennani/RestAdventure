using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     An action performed by a character
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type", IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(MoveActionDto), "move")]
[JsonDerivedType(typeof(HarvestActionDto), "harvest")]
[JsonDerivedType(typeof(StartPveCombatActionDto), "start-combat-pve")]
[JsonDerivedType(typeof(PveCombatActionDto), "combat-pve")]
public class ActionDto
{
    /// <summary>
    ///     The name of the action
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class CharacterActionMappingExtensions
{
    public static ActionDto ToDto(this Action action) =>
        action switch
        {
            MoveAction moveAction => moveAction.ToDto(),
            HarvestAction harvestAction => harvestAction.ToDto(),
            StartPveCombatAction startPveCombatAction => startPveCombatAction.ToDto(),
            PveCombatAction pveCombatAction => pveCombatAction.ToDto(),
            _ => new ActionDto { Name = action.Name }
        };
}
