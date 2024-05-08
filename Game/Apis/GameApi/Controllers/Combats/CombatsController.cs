using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Game.Apis.Common.Dtos.Combats;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Combats;

/// <summary>
///     Combats operations
/// </summary>
[Route("game/location/{locationGuid:guid}/combats")]
[OpenApiTag("Combats")]
public class CombatsController : GameApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public CombatsController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Get combats in preparation
    /// </summary>
    [HttpGet("in-preparation")]
    public ActionResult<IReadOnlyCollection<CombatInPreparationDto>> GetCombatsInPreparation(Guid locationGuid)
    {
        GameState state = _gameService.RequireGameState();

        LocationId locationId = new(locationGuid);
        Location? location = state.Content.Maps.Locations.Get(locationId);
        if (location == null)
        {
            return NotFound();
        }

        IEnumerable<CombatInPreparation> combats = state.Combats.InPreparationAtLocation(location);
        return combats.Select(c => c.ToDto()).ToArray();
    }

    /// <summary>
    ///     Get combats
    /// </summary>
    [HttpGet]
    public ActionResult<IReadOnlyCollection<CombatInstanceDto>> GetCombats(Guid locationGuid)
    {
        GameState state = _gameService.RequireGameState();

        LocationId locationId = new(locationGuid);
        Location? location = state.Content.Maps.Locations.Get(locationId);
        if (location == null)
        {
            return NotFound();
        }

        IEnumerable<CombatInstance> combats = state.Combats.AtLocation(location);
        return combats.Select(c => c.ToDto()).ToArray();
    }
}
