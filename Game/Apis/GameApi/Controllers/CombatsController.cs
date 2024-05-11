using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Players;
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
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);
        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IEnumerable<CombatInstance> combats = state.Combats.GetCombatAtLocation(character.Location);
        return combats.Select(c => c.ToDto()).ToArray();
    }
}
