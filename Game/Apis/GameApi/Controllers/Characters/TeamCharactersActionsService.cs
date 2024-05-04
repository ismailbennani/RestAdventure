using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Core.Maps;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

[Route("game/team/characters/{characterId:guid}")]
[OpenApiTag("Team")]
public class TeamCharactersActionsService : GameApiController
{
    readonly GameService _gameService;
    readonly CharacterActionsService _characterActionsService;

    public TeamCharactersActionsService(GameService gameService, CharacterActionsService characterActionsService)
    {
        _gameService = gameService;
        _characterActionsService = characterActionsService;
    }

    /// <summary>
    ///     Move to location
    /// </summary>
    [HttpPost("move/{locationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult MoveToLocation(Guid characterId, Guid locationId)
    {
        Guid playerId = ControllerContext.RequirePlayerId();
        GameState state = _gameService.RequireGameState();

        Team? team = state.Characters.GetTeamsOfPlayer(playerId).FirstOrDefault();
        if (team == null)
        {
            return NotFound();
        }

        Character? character = state.Characters.GetCharacter(team, characterId);
        if (character == null)
        {
            return NotFound();
        }

        MapLocation? location = state.Map.GetLocation(locationId);
        if (location == null)
        {
            return NotFound();
        }

        _characterActionsService.MoveToLocation(character, location);

        return NoContent();
    }
}
