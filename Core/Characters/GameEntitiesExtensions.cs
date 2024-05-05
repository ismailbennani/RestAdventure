using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Characters;

public static class GameEntitiesExtensions
{
    public static Character RequireCharacter(this GameEntities entities, CharacterId id) =>
        entities.Get<Character>(id) ?? throw new InvalidOperationException($"Could not find character {id}");
}
