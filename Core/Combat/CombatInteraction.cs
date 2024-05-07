using RestAdventure.Core.Characters;
using RestAdventure.Core.Interactions;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public abstract class CombatInteraction : Interaction
{
    public override string Name => "combat";

    public override async Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(GameState state, Character character, IInteractibleEntity target)
    {
        IGameEntityWithCombatStatistics entityWithCombat = (IGameEntityWithCombatStatistics)target;

        CombatInstance combat = await state.Combats.StartCombatAsync(new CombatFormation { Entities = [character] }, new CombatFormation { Entities = [entityWithCombat] });

        return new CharacterCombatInteractionInstance(combat, character, this, target);
    }
}
