using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Interactions;

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

    public override CharacterActionResolution Perform(GameContent content, GameState state, Character character)
    {
        if (!state.Interactions.TryStartInteraction(character, Interaction, Entity, out string? whyNot))
        {
            return new CharacterActionResolution { Success = false, ErrorMessage = whyNot };
        }

        return new CharacterActionResolution { Success = true };
    }
}
