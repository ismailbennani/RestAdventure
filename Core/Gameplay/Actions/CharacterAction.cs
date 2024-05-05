using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Gameplay.Actions;

public abstract class CharacterAction
{
    protected CharacterAction(Character character)
    {
        CharacterId = character.Id;
    }

    protected CharacterId CharacterId { get; }

    public abstract CharacterActionResolution Perform(GameContent content, GameState state);
}
