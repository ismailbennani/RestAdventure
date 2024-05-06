using RestAdventure.Core.Players;

namespace RestAdventure.Core.Characters.Services;

public class CharactersService
{
    readonly GameService _gameService;

    public CharactersService(GameService gameService)
    {
        _gameService = gameService;
    }

    public async Task<CharacterCreationResult> CreateCharacterAsync(Player player, string name, CharacterClass cls)
    {
        GameState state = _gameService.RequireGameState();

        int maxTeamSize = state.Settings.MaxTeamSize;
        IEnumerable<Character> characters = state.Entities.GetCharactersOfPlayer(player);
        if (characters.Count() >= maxTeamSize)
        {
            return new CharacterCreationResult { IsSuccess = false, ErrorMessage = $"reached max team size (max:{maxTeamSize})" };
        }

        Character character = new(player, cls, name);
        await state.Entities.RegisterAsync(character);

        return new CharacterCreationResult { IsSuccess = true, Character = character };
    }
}
