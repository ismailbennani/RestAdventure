using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Characters;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

/// <summary>
///     Team operations
/// </summary>
[Route("game/team")]
[OpenApiTag("Team")]
public class TeamController : GameApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public TeamController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Get team
    /// </summary>
    [HttpGet]
    public ActionResult<TeamDto> GetTeam()
    {
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        IEnumerable<Character> characters = state.Entities.GetCharactersOfPlayer(player);

        return new TeamDto
        {
            Characters = characters.ToList()
                .Select(
                    c => c.ToDto(
                        new CharacterMappingOptions
                        {
                            LastActionResult = state.Actions.GetLastActionResult(c),
                            NextAction = state.Actions.GetNextAction(c)
                        }
                    )
                )
                .ToArray()
        };
    }
}
