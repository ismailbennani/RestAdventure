using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Game.Apis.GameApi.Controllers.Characters.Requests;
using RestAdventure.Game.Apis.GameApi.Dtos.Characters;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

[Route("game/team/characters")]
[OpenApiTag("Team")]
public class TeamCharactersController : GameApiController
{
    readonly GameService _gameService;

    public TeamCharactersController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Create character
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TeamCharacterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<TeamCharacterDto> CreateCharacter(CreateCharacterRequestDto request)
    {
        Guid playerId = ControllerContext.RequirePlayerId();
        GameState state = _gameService.RequireGameState();

        Team team = GetOrCreateTeam(state, playerId);
        CharacterCreationResult result = state.Characters.CreateCharacter(team, request.Name, request.Class);

        if (!result.IsSuccess)
        {
            return Problem($"Could not create character: {result.ErrorMessage}", statusCode: StatusCodes.Status400BadRequest);
        }

        return result.Character.ToDto();
    }

    /// <summary>
    ///     Delete character
    /// </summary>
    [HttpDelete("{characterId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult DeleteCharacter(Guid characterId)
    {
        Guid playerId = ControllerContext.RequirePlayerId();
        GameState state = _gameService.RequireGameState();

        Team? team = state.Characters.GetTeam(playerId);
        if (team == null)
        {
            return NotFound();
        }

        Character? character = state.Characters.GetCharacter(team, playerId);
        if (character == null)
        {
            return NotFound();
        }

        state.Characters.DeleteCharacter(team, characterId);

        return NoContent();
    }

    static Team GetOrCreateTeam(GameState state, Guid playerId) => state.Characters.GetTeamsOfPlayer(playerId).FirstOrDefault() ?? state.Characters.CreateTeam(playerId);
}
