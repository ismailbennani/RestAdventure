namespace RestAdventure.Core.Characters;

public class CharacterInteractionsService
{
    public IEnumerable<Character> GetCharactersInRange(Character character) => character.Location.Characters;
    public IEnumerable<Character> GetCharactersInRange(IEnumerable<Character> characters) => characters.SelectMany(c => c.Location.Characters).Distinct();
}

public static class CharactersInteractionServiceExtensions
{
    public static IEnumerable<Character> GetCharactersInRange(this CharacterInteractionsService service, Team team) => service.GetCharactersInRange(team.Characters);
}
