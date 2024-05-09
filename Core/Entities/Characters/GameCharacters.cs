using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Entities.Characters;

public class GameCharacters
{
    public GameResourcesStore<CharacterClassId, CharacterClass> Classes { get; } = new();
}
