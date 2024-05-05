namespace RestAdventure.Core.Characters;

public class CharacterInteractionsService
{
    readonly GameService _gameService;

    public CharacterInteractionsService(GameService gameService)
    {
        _gameService = gameService;
    }

    public IEnumerable<Character> GetCharactersInRange(Character character)
    {
        GameState state = _gameService.RequireGameState();
        return state.Characters.GetCharactersAtLocation(character.Location);
    }

    public IEnumerable<Character> GetCharactersInRange(IEnumerable<Character> characters)
    {
        GameState state = _gameService.RequireGameState();
        return characters.SelectMany(c => state.Characters.GetCharactersAtLocation(c.Location)).Distinct();
    }
}

public static class CharactersInteractionServiceExtensions
{
    public static IEnumerable<Character> GetCharactersInRange(this CharacterInteractionsService service, Team team) => service.GetCharactersInRange(team.Characters);
}
