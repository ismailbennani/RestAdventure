using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Maps;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Locations operations
/// </summary>
[Route("game/team/characters/{characterGuid:guid}/locations")]
[OpenApiTag("Locations")]
public class LocationsController : GameApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public LocationsController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Get accessible locations
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<LocationWithAccessDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyCollection<LocationWithAccessDto>> GetAccessibleLocations(Guid characterGuid)
    {
        GameSnapshot state = _gameService.GetLastSnapshot();
        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        IEnumerable<MoveAction> actions = state.Actions.GetAvailableActions(state, character).OfType<MoveAction>();

        List<LocationWithAccessDto> result = [];
        foreach (MoveAction action in actions)
        {
            Maybe canMove = character.Busy ? "Character is busy" : true;
            result.Add(
                new LocationWithAccessDto
                {
                    Location = action.Location.ToMinimalDto(),
                    IsAccessible = canMove.Success,
                    WhyIsNotAccessible = canMove.WhyNot
                }
            );
        }

        return result;
    }

    /// <summary>
    ///     Move to location
    /// </summary>
    [HttpPost("{locationGuid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult MoveToLocation(Guid characterGuid, Guid locationGuid)
    {
        GameSnapshot state = _gameService.GetLastSnapshot();
        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        MoveAction? action = state.Actions.GetAvailableActions(state, character).OfType<MoveAction>().SingleOrDefault(a => a.Location.Id.Guid == locationGuid);
        if (action == null)
        {
            return NotFound();
        }

        Core.Game game = _gameService.RequireGame();
        Character? gameCharacter = game.Entities.Get<Character>(characterId);
        if (gameCharacter == null)
        {
            return BadRequest();
        }

        game.Actions.QueueAction(gameCharacter, action);

        return NoContent();
    }
}
