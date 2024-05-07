using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Gameplay.Actions;

public class CharacterInteractWithEntityAction : CharacterAction
{
    public CharacterInteractWithEntityAction(Interaction interaction, IGameEntityWithInteractions entity)
    {
        Interaction = interaction;
        Entity = entity;
    }

    public Interaction Interaction { get; }
    public IGameEntityWithInteractions Entity { get; }

    public override async Task<Maybe> PerformAsync(GameState state, Character character) => await state.Interactions.StartInteractionAsync(character, Interaction, Entity);
}
