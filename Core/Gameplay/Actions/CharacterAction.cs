using RestAdventure.Core.Characters;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Gameplay.Actions;

public abstract class CharacterAction
{
    public abstract Task<Maybe> PerformAsync(GameState state, Character character);
}
