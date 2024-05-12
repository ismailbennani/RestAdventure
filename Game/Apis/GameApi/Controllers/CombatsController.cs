using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Combats;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Combats;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Combats operations
/// </summary>
[Route("game/team/characters/{characterGuid:guid}/combats")]
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
    ///     Get combats
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<CombatDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyCollection<CombatDto>> GetCombats(Guid characterGuid)
    {
        GameSnapshot state = _gameService.GetLastSnapshot();
        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        IEnumerable<CombatInstanceSnapshot> combats = state.Combats.Values.Where(c => c.Location == character.Location);
        return combats.Select(c => c.ToDto()).ToArray();
    }
}
