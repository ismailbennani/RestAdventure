using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Content;
using RestAdventure.Game.Apis.Common.Dtos.Characters;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.Jobs;
using RestAdventure.Game.Apis.Common.Dtos.Maps;
using RestAdventure.Game.Apis.Common.Dtos.Queries;
using RestAdventure.Game.Apis.Common.Dtos.StaticObjects;
using RestAdventure.Kernel.Queries;

namespace RestAdventure.Game.Apis.AdminApi.Controllers;

/// <summary>
///     Game content admin operations
/// </summary>
[Route("admin/game/content")]
[OpenApiTag("Game")]
public class AdminGameContentController : AdminApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public AdminGameContentController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Search character classes
    /// </summary>
    [HttpGet("characters/classes")]
    public SearchResultDto<CharacterClassDto> SearchCharacterClasses([FromQuery] SearchRequestDto request) =>
        SearchResources(request, content => content.Characters.Classes, (_, i) => i.ToDto());

    /// <summary>
    ///     Search items
    /// </summary>
    [HttpGet("items")]
    public SearchResultDto<ItemDto> SearchItems([FromQuery] SearchRequestDto request) => SearchResources(request, content => content.Items, (_, i) => i.ToDto());

    /// <summary>
    ///     Search locations
    /// </summary>
    [HttpGet("locations")]
    public SearchResultDto<LocationDto> SearchLocations([FromQuery] SearchRequestDto request) =>
        SearchResources(request, content => content.Maps.Locations, (content, l) => l.ToDiscoveredLocationDto(content));

    /// <summary>
    ///     Search jobs
    /// </summary>
    [HttpGet("jobs")]
    public SearchResultDto<JobDto> SearchJobs([FromQuery] SearchRequestDto request) => SearchResources(request, content => content.Jobs, (_, j) => j.ToDto());

    /// <summary>
    ///     Search static objects
    /// </summary>
    [HttpGet("static-objects")]
    public SearchResultDto<StaticObjectDto> SearchHarvestables([FromQuery] SearchRequestDto request) =>
        SearchResources(request, content => content.StaticObjects, (_, h) => h.ToDto());

    SearchResultDto<TDto> SearchResources<TResource, TDto>(
        [FromQuery] SearchRequestDto request,
        Func<GameContent, IEnumerable<TResource>> getResources,
        Func<GameContent, TResource, TDto> selector
    )
    {
        GameContent content = _gameService.RequireGameContent();
        IEnumerable<TResource> harvestables = getResources(content);
        return Search.Paginate(harvestables, request.ToPaginationParameters()).ToDto(r => selector(content, r));
    }
}
