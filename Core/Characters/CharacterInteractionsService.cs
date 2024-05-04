namespace RestAdventure.Core.Characters;

public class CharacterInteractionsService
{
    public IQueryable<CharacterDbo> GetCharactersInRange(CharacterDbo character) => character.Location.Characters;
    public IQueryable<CharacterDbo> GetCharactersInRange(IQueryable<CharacterDbo> characters) => characters.SelectMany(c => c.Location.Characters).Distinct();
}

public static class CharactersInteractionServiceExtensions
{
    public static IQueryable<CharacterDbo> GetCharactersInRange(this CharacterInteractionsService service, TeamDbo team) => service.GetCharactersInRange(team.Characters);
}
