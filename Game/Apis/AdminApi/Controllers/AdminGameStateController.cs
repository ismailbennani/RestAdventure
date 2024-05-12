using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Queries;
using RestAdventure.Kernel.Queries;

namespace RestAdventure.Game.Apis.AdminApi.Controllers;

/// <summary>
///     Game state admin operations
/// </summary>
[Route("admin/game")]
[OpenApiTag("Game")]
public class AdminGameStateController : AdminApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public AdminGameStateController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Search harvestable instances
    /// </summary>
    [HttpGet("static-objects/{staticObjectGuid:guid}")]
    public SearchResultDto<EntityDto> SearchStaticObjectInstances(Guid staticObjectGuid, [FromQuery] SearchRequestDto request)
    {
        GameSnapshot state = _gameService.GetLastSnapshot();

        StaticObjectId staticObjectId = new(staticObjectGuid);
        IEnumerable<StaticObjectInstanceSnapshot> instances = state.Entities.Values.OfType<StaticObjectInstanceSnapshot>().Where(o => o.Object.Id == staticObjectId);

        return Search.Paginate(instances, request.ToPaginationParameters()).ToDto(instance => instance.ToDto());
    }
}
