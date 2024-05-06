using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Gameplay.Actions;

public abstract class CharacterAction
{
    public abstract CharacterActionResolution Perform(GameContent content, GameState state, Character character);
}
