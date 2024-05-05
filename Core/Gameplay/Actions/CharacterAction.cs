using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Gameplay.Actions;

public abstract class CharacterAction
{
    protected CharacterAction(Character character)
    {
        Character = character;
    }

    protected Character Character { get; }

    public abstract CharacterActionResolution Perform(GameContent content, GameState state);
}
