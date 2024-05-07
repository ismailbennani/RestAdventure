using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public abstract class CombatInteraction : Interaction
{
    public override string Name => "combat";

    public override async Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(GameState state, Character character, IGameEntity entity)
    {
        IGameEntityWithCombatStatistics target = (IGameEntityWithCombatStatistics)entity;

        CombatInstance combat = await state.Combats.StartCombatAsync(new CombatFormation { Entities = [character] }, new CombatFormation { Entities = [target] });

        return new CharacterCombatInteractionInstance(combat, character, this, entity);
    }
}

public class CharacterCombatInteractionInstance : InteractionInstance
{
    public CombatInstance Combat { get; }

    public CharacterCombatInteractionInstance(CombatInstance combat, Character character, Interaction interaction, IGameEntity subject) : base(character, interaction, subject)
    {
        Combat = combat;
    }

    public override bool IsOver(GameState state) => Combat.IsOver;

    public override async Task OnTickAsync(GameState state) => await Combat.PlayTurnAsync();
}
