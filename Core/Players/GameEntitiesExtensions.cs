using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Players;

public static class GameEntitiesExtensions
{
    public static IEnumerable<Character> GetCharactersOfPlayer(this GameEntities entities, Player player) => entities.All.OfType<Character>().Where(c => c.Player == player);
}
