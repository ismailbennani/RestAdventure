using RestAdventure.Core.Characters;
using RestAdventure.Core.Interactions;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Combat;

public class CombatInteraction : Interaction
{
    public override string Name => "Combat";

    public override Task<Maybe> CanInteractAsync(GameState state, Character character, IGameEntityWithInteractions entity)
    {
        if (entity is not IGameEntityWithCombatStatistics target)
        {
            return Task.FromResult<Maybe>($"Expected target to be a {nameof(IGameEntityWithCombatStatistics)} but found {entity.GetType()}");
        }

        if (character.Location != target.Location)
        {
            return Task.FromResult<Maybe>("Target is inaccessible");
        }

        return Task.FromResult<Maybe>(true);
    }

    public override async Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(GameState state, Character character, IGameEntityWithInteractions entity)
    {
        IGameEntityWithCombatStatistics target = (IGameEntityWithCombatStatistics)entity;

        CombatInstance combat = await state.Combats.StartCombatAsync(new CombatFormation { Entities = [character] }, new CombatFormation { Entities = [target] });

        return new CharacterCombatInteractionInstance(combat, character, this, entity);
    }
}

public class CharacterCombatInteractionInstance : InteractionInstance
{
    public CombatInstance Combat { get; }

    public CharacterCombatInteractionInstance(CombatInstance combat, Character character, Interaction interaction, IGameEntityWithInteractions subject) : base(
        character,
        interaction,
        subject
    )
    {
        Combat = combat;
    }

    public override bool IsOver(GameState state) => Combat.IsOver;

    public override async Task OnTickAsync(GameState state) => await Combat.PlayTurnAsync();
}
