using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Interactions;

namespace RestAdventure.Core.Gameplay.Actions;

public class CharacterInteractWithEntityAction : CharacterAction
{
    public CharacterInteractWithEntityAction(Character character, Interaction interaction, IGameEntityWithInteractions entity) : base(character)
    {
        Interaction = interaction;
        Entity = entity;
    }

    public Interaction Interaction { get; }
    public IGameEntityWithInteractions Entity { get; }

    public override CharacterActionResolution Perform(GameContent content, GameState state)
    {
        if (!state.Interactions.TryStartInteraction(Character, Interaction, Entity, out string? whyNot))
        {
            return new CharacterActionResolution { Success = false, ErrorMessage = whyNot };
        }

        return new CharacterActionResolution { Success = true };
    }
}
