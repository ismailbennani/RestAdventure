using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.GameApi.Dtos.Characters;
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
    readonly CharacterActionsService _characterActionsService;

    /// <summary>
    /// </summary>
    public TeamController(GameService gameService, CharacterActionsService characterActionsService)
    {
        _gameService = gameService;
        _characterActionsService = characterActionsService;
    }

    /// <summary>
    ///     Get team
    /// </summary>
    [HttpGet]
    public ActionResult<TeamDto> GetTeam()
    {
        PlayerId playerId = ControllerContext.RequirePlayerId();
        GameState state = _gameService.RequireGameState();

        Team team = state.Characters.GetTeams(playerId).FirstOrDefault() ?? state.Characters.CreateTeam(playerId);

        return new TeamDto
        {
            Characters = team.Characters.ToList()
                .Select(
                    c => c.ToDto(
                        new CharacterMappingOptions
                        {
                            LastActionResult = _characterActionsService.GetLastActionResult(c),
                            NextAction = _characterActionsService.GetNextAction(c)
                        }
                    )
                )
                .ToArray()
        };
    }
}
