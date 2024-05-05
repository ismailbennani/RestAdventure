using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Items;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.GameApi.Dtos.Items;
using RestAdventure.Game.Apis.GameApi.Dtos.Maps;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Security;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Game content operations
/// </summary>
[Route("game/content")]
[OpenApiTag("Game")]
public class GameContentController : GameApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public GameContentController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Get item
    /// </summary>
    [HttpGet("items/{itemId:guid}")]
    [ProducesResponseType<ItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<ItemDto> GetItem(Guid itemId)
    {
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();

        UserId userId = ControllerContext.RequireUserId();
        Player? player = state.Players.GetPlayer(userId);
        if (player == null)
        {
            return BadRequest();
        }

        Item? item = content.Items.Get(new ItemId(itemId));
        if (item == null)
        {
            return NotFound();
        }

        if (!player.HasDiscovered(item))
        {
            return NotFound();
        }

        return item.ToDto();
    }

    /// <summary>
    ///     Get location
    /// </summary>
    [HttpGet("locations/{locationId:guid}")]
    [ProducesResponseType<ItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<MapLocationDto> GetLocation(Guid locationId)
    {
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();

        UserId userId = ControllerContext.RequireUserId();
        Player? player = state.Players.GetPlayer(userId);
        if (player == null)
        {
            return BadRequest();
        }

        MapLocation? location = content.Maps.Locations.Get(new MapLocationId(locationId));
        if (location == null)
        {
            return NotFound();
        }

        if (!player.HasDiscovered(location))
        {
            return NotFound();
        }

        return location.ToDiscoveredLocationDto(content);
    }
}
