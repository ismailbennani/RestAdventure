using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Characters;
using RestAdventure.Game.Apis.GameApi.Controllers.Characters.Requests;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

/// <summary>
///     Team characters operations
/// </summary>
[Route("game/team/characters")]
[OpenApiTag("Team")]
public class TeamCharactersController : GameApiController
{
    readonly GameService _gameService;
    readonly CharactersService _charactersService;

    /// <summary>
    /// </summary>
    public TeamCharactersController(GameService gameService, CharactersService charactersService)
    {
        _gameService = gameService;
        _charactersService = charactersService;
    }

    /// <summary>
    ///     Create character
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TeamCharacterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamCharacterDto>> CreateCharacterAsync(CreateCharacterRequestDto request)
    {
        GameState state = _gameService.RequireGameState();
        GameContent content = _gameService.RequireGameContent();

        Player player = ControllerContext.RequirePlayer(state);
        CharacterCreationResult result = await _charactersService.CreateCharacterAsync(player, request.Name, request.Class);

        if (!result.IsSuccess)
        {
            return Problem($"Could not create character: {result.ErrorMessage}", statusCode: StatusCodes.Status400BadRequest);
        }

        return result.Character.ToDto(content);
    }

    /// <summary>
    ///     Delete character
    /// </summary>
    [HttpDelete("{characterGuid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCharacterAsync(Guid characterGuid)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);
        if (character == null || character.Player != player)
        {
            return NotFound();
        }

        await state.Entities.UnregisterAsync(character);

        return NoContent();
    }
}
