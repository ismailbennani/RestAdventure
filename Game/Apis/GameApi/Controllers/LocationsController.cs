using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Content;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Players;
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
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IEnumerable<MoveAction> actions = state.Actions.GetAvailableActions(character).OfType<MoveAction>();

        List<LocationWithAccessDto> result = [];
        foreach (MoveAction action in actions)
        {
            Maybe canMove = action.CanPerform(state, character);
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
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        MoveAction? action = state.Actions.GetAvailableActions(character).OfType<MoveAction>().SingleOrDefault(a => a.Location.Id.Guid == locationGuid);
        if (action == null)
        {
            return NotFound();
        }

        state.Actions.QueueAction(character, action);

        return NoContent();
    }
}
