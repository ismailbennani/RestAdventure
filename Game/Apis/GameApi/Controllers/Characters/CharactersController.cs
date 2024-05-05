using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Characters;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

/// <summary>
///     Characters operations
/// </summary>
[Route("game/characters")]
[OpenApiTag("Characters")]
public class CharactersController : GameApiController
{
    readonly GameService _gameService;
    readonly CharacterInteractionsService _characterInteractionsService;

    /// <summary>
    /// </summary>
    public CharactersController(GameService gameService, CharacterInteractionsService characterInteractionsService)
    {
        _gameService = gameService;
        _characterInteractionsService = characterInteractionsService;
    }

    /// <summary>
    ///     Get characters in range
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CharacterMinimalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyCollection<CharacterMinimalDto>> GetCharactersInRange()
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        Team? team = state.Characters.GetTeams(player).FirstOrDefault();
        if (team == null)
        {
            return NotFound();
        }

        return _characterInteractionsService.GetCharactersInRange(team).Select(c => c.ToMinimalCharacterDto()).ToArray();
    }
}
