using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Maps.Locations;
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

        IEnumerable<Location> accessibleLocations = content.Maps.Locations.ConnectedTo(character.Location);

        List<LocationWithAccessDto> result = [];
        foreach (Location location in accessibleLocations)
        {
            Maybe canMove = state.CharacterActions.CanMoveTo(character, location);
            result.Add(
                new LocationWithAccessDto
                {
                    Location = location.ToMinimalDto(),
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

        LocationId locationId = new(locationGuid);
        Location? location = content.Maps.Locations.Get(locationId);
        if (location == null)
        {
            return NotFound();
        }

        state.CharacterActions.PlanMovement(character, location);

        return NoContent();
    }
}
