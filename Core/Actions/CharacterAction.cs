using RestAdventure.Core.Characters;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Actions;

public abstract class CharacterAction
{
    public Maybe CanPerform(GameState state, Character character)
    {
        if (character.CurrentInteraction != null)
        {
            return "Character is busy";
        }

        return CanPerformInternal(state, character);
    }

    protected abstract Maybe CanPerformInternal(GameState state, Character character);
    public abstract Task<Maybe> PerformAsync(GameState state, Character character);
}
