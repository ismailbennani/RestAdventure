using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Gameplay.Actions;

public abstract class CharacterAction
{
    protected CharacterAction(Character character)
    {
        TeamId = character.Team.Id;
        CharacterId = character.Id;
    }

    protected TeamId TeamId { get; }
    protected CharacterId CharacterId { get; }

    public abstract CharacterActionResolution Perform(GameContent content, GameState state);
}
