using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;

namespace RestAdventure.Core.Players;

public static class GameEntitiesExtensions
{
    public static IEnumerable<Character> GetCharactersOfPlayer(this GameEntities entities, Player player) => entities.OfType<Character>().Where(c => c.Player == player);
}
