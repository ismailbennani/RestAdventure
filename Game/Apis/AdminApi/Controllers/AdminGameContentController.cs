using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Items;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Game.Apis.Common.Dtos.Queries;
using RestAdventure.Game.Apis.GameApi.Dtos.Items;
using RestAdventure.Game.Apis.GameApi.Dtos.Maps;
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
    public SearchResult<ItemDto> SearchItems([FromQuery] SearchRequestDto request)
    {
        GameContent content = _gameService.RequireGameContent();
        IEnumerable<Item> items = content.Items.All;
        return Search.Paginate(items, request.ToPaginationParameters()).Select(i => i.ToDto());
    }

    /// <summary>
    ///     Search locations
    /// </summary>
    [HttpGet("locations")]
    public SearchResult<MapLocationDto> SearchLocations([FromQuery] SearchRequestDto request)
    {
        GameContent content = _gameService.RequireGameContent();
        IEnumerable<MapLocation> locations = content.Maps.Locations.All;
        return Search.Paginate(locations, request.ToPaginationParameters()).Select(i => i.ToDiscoveredLocationDto(content));
    }
}
