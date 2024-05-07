using RestAdventure.Core.Characters;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Interactions;

public abstract class Interaction
{
    /// <summary>
    ///     The unique name of the interaction
    /// </summary>
    public abstract string Name { get; }

    public async Task<Maybe> CanInteractAsync(GameState state, Character character, IInteractibleEntity target)
    {
        InteractionInstance? ongoingInteraction = state.Interactions.GetCharacterInteraction(character);
        if (ongoingInteraction != null)
        {
            return "Character is busy";
        }

        if (target.Disabled)
        {
            return "Target is busy";
        }

        if (character.Location != target.Location)
        {
            return "Target is inaccessible";
        }

        return await CanInteractInternalAsync(state, character, target);
    }

    protected abstract Task<Maybe> CanInteractInternalAsync(GameState state, Character character, IInteractibleEntity target);
    public abstract Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(GameState state, Character character, IInteractibleEntity target);

    public override string ToString() => Name;
}
