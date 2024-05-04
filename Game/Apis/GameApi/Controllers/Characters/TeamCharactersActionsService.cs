using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Players;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

[Route("game/team/characters/{characterGuid:guid}")]
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
    [HttpPost("move/{locationGuid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult MoveToLocation(Guid characterGuid, Guid locationGuid)
    {
        PlayerId playerId = ControllerContext.RequirePlayerId();
        GameState state = _gameService.RequireGameState();
        CharacterId characterId = new(characterGuid);
        MapLocationId locationId = new(locationGuid);

        Team? team = state.Characters.GetTeams(playerId).FirstOrDefault();
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
