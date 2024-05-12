using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Resources;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Players;
using RestAdventure.Game.Apis.Common.Dtos.Characters;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.Jobs;
using RestAdventure.Game.Apis.Common.Dtos.Maps;
using RestAdventure.Game.Apis.Common.Dtos.StaticObjects;
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
    ///     Get character class
    /// </summary>
    [HttpGet("characters/classes/{characterClassId:guid}")]
    [ProducesResponseType<CharacterClassDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<CharacterClassDto> GetCharacterClass(Guid characterClassId) =>
        GetResource(content => content.Characters.Classes.Get(new CharacterClassId(characterClassId)), (_, c) => c.ToDto());

    /// <summary>
    ///     Get item
    /// </summary>
    [HttpGet("items/{itemId:guid}")]
    [ProducesResponseType<ItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<ItemDto> GetItem(Guid itemId) => GetResource(content => content.Items.Get(new ItemId(itemId)), (_, j) => j.ToDto());

    /// <summary>
    ///     Get location
    /// </summary>
    [HttpGet("locations/{locationId:guid}")]
    [ProducesResponseType<LocationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<LocationDto> GetLocation(Guid locationId) =>
        GetResource(content => content.Maps.Locations.Get(new LocationId(locationId)), (content, l) => l.ToDiscoveredLocationDto(content));

    /// <summary>
    ///     Get job
    /// </summary>
    [HttpGet("jobs/{jobId:guid}")]
    [ProducesResponseType<JobDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<JobDto> GetJob(Guid jobId) => GetResource(content => content.Jobs.Get(new JobId(jobId)), (_, j) => j.ToDto());

    /// <summary>
    ///     Get static object
    /// </summary>
    [HttpGet("static-objects/{staticObjectId:guid}")]
    [ProducesResponseType<StaticObjectDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<StaticObjectDto> GetHarvestable(Guid staticObjectId) =>
        GetResource(content => content.StaticObjects.Get(new StaticObjectId(staticObjectId)), (_, h) => h.ToStaticObjectDto());

    ActionResult<TDto> GetResource<TResource, TDto>(Func<GameContent, TResource?> findResource, Func<GameContent, TResource, TDto> map) where TResource: GameResource
    {
        GameSnapshot state = _gameService.GetLastSnapshot();

        UserId userId = ControllerContext.RequireUserId();
        PlayerSnapshot? player = state.Players.GetValueOrDefault(userId);
        if (player == null)
        {
            return BadRequest();
        }

        GameContent content = _gameService.RequireGameContent();
        TResource? resource = findResource(content);
        if (resource == null)
        {
            return NotFound();
        }

        if (!player.Knowledge.Contains(resource.Id))
        {
            return NotFound();
        }

        return map(content, resource);
    }
}
