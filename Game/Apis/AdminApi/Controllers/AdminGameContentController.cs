using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Harvestables;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Game.Apis.Common.Dtos.Harvestables;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.Jobs;
using RestAdventure.Game.Apis.Common.Dtos.Maps;
using RestAdventure.Game.Apis.Common.Dtos.Queries;
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
    ///     Search items
    /// </summary>
    [HttpGet("items")]
    public SearchResultDto<ItemDto> SearchItems([FromQuery] SearchRequestDto request)
    {
        GameContent content = _gameService.RequireGameContent();
        IEnumerable<Item> items = content.Items.All;
        return Search.Paginate(items, request.ToPaginationParameters()).ToDto(i => i.ToDto());
    }

    /// <summary>
    ///     Search locations
    /// </summary>
    [HttpGet("locations")]
    public SearchResultDto<LocationDto> SearchLocations([FromQuery] SearchRequestDto request)
    {
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();
        IEnumerable<Location> locations = content.Maps.Locations.All;
        return Search.Paginate(locations, request.ToPaginationParameters()).ToDto(i => i.ToDiscoveredLocationDto(content));
    }

    /// <summary>
    ///     Search jobs
    /// </summary>
    [HttpGet("jobs")]
    public SearchResultDto<JobDto> SearchJobs([FromQuery] SearchRequestDto request)
    {
        GameContent content = _gameService.RequireGameContent();
        IEnumerable<Job> jobs = content.Jobs.All;
        return Search.Paginate(jobs, request.ToPaginationParameters()).ToDto(i => i.ToDto());
    }

    /// <summary>
    ///     Search harvestables
    /// </summary>
    [HttpGet("harvestables")]
    public SearchResultDto<HarvestableDto> SearchHarvestables([FromQuery] SearchRequestDto request)
    {
        GameContent content = _gameService.RequireGameContent();
        IEnumerable<Harvestable> harvestables = content.Harvestables.All;
        return Search.Paginate(harvestables, request.ToPaginationParameters()).ToDto(i => i.ToDto());
    }
}
