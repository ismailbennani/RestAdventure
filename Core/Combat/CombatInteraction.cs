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

public class CharacterCombatInteractionInstance : InteractionInstance
{
    public CombatInstance Combat { get; }

    public CharacterCombatInteractionInstance(CombatInstance combat, Character character, Interaction interaction, IInteractibleEntity target) : base(
        character,
        interaction,
        target
    )
    {
        Combat = combat;
    }

    public override bool IsOver(GameState state) => Combat.IsOver;

    public override async Task OnTickAsync(GameState state) => await Combat.PlayTurnAsync();
}
