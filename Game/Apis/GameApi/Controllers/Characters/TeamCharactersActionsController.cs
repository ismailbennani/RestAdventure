using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Players;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

/// <summary>
///     Characters actions operations
/// </summary>
[Route("game/team/characters/{characterGuid:guid}")]
[OpenApiTag("Team")]
public class TeamCharactersActionsController : GameApiController
{
    readonly GameService _gameService;
    readonly CharacterActionsService _characterActionsService;

    /// <summary>
    /// </summary>
    public TeamCharactersActionsController(GameService gameService, CharacterActionsService characterActionsService)
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
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        Team? team = state.Characters.GetTeams(player).FirstOrDefault();
        if (team == null)
        {
            return NotFound();
        }

        CharacterId characterId = new(characterGuid);
        Character? character = state.Characters.GetCharacter(team, characterId);
        if (character == null)
        {
            return NotFound();
        }

        MapLocationId locationId = new(locationGuid);
        MapLocation? location = content.Maps.Locations.Get(locationId);
        if (location == null)
        {
            return NotFound();
        }

        _characterActionsService.MoveToLocation(character, location);

        return NoContent();
    }
}
