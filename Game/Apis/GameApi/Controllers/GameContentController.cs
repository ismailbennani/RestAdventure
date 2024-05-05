using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Harvestables;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Harvestables;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.Jobs;
using RestAdventure.Game.Apis.Common.Dtos.Maps;
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

        if (!player.Knowledge.Knows(item))
        {
            return NotFound();
        }

        return item.ToDto();
    }

    /// <summary>
    ///     Get location
    /// </summary>
    [HttpGet("locations/{locationId:guid}")]
    [ProducesResponseType<MapLocationDto>(StatusCodes.Status200OK)]
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

        Location? location = content.Maps.Locations.Get(new LocationId(locationId));
        if (location == null)
        {
            return NotFound();
        }

        if (!player.Knowledge.Knows(location))
        {
            return NotFound();
        }

        return location.ToDiscoveredLocationDto(content);
    }

    /// <summary>
    ///     Get job
    /// </summary>
    [HttpGet("jobs/{jobId:guid}")]
    [ProducesResponseType<JobDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<JobDto> GetJob(Guid jobId)
    {
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();

        UserId userId = ControllerContext.RequireUserId();
        Player? player = state.Players.GetPlayer(userId);
        if (player == null)
        {
            return BadRequest();
        }

        Job? job = content.Jobs.Get(new JobId(jobId));
        if (job == null)
        {
            return NotFound();
        }

        if (!player.Knowledge.Knows(job))
        {
            return NotFound();
        }

        return job.ToDto();
    }

    /// <summary>
    ///     Get harvestable
    /// </summary>
    [HttpGet("harvestables/{harvestableId:guid}")]
    [ProducesResponseType<HarvestableDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<HarvestableDto> GetHarvestable(Guid harvestableId)
    {
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();

        UserId userId = ControllerContext.RequireUserId();
        Player? player = state.Players.GetPlayer(userId);
        if (player == null)
        {
            return BadRequest();
        }

        Harvestable? harvestable = content.Harvestables.Get(new HarvestableId(harvestableId));
        if (harvestable == null)
        {
            return NotFound();
        }

        if (!player.Knowledge.Knows(harvestable))
        {
            return NotFound();
        }

        return harvestable.ToDto();
    }
}
