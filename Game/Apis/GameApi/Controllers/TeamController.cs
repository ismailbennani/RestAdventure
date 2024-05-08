using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Characters;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

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
                            OngoingAction = state.Actions.GetOngoingAction(c),
                            PlannedAction = state.Actions.GetQueuedAction(c)
                        }
                    )
                )
                .ToArray()
        };
    }
}
