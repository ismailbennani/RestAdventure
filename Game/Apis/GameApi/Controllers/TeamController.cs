using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;
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
        GameSnapshot state = _gameService.GetLastSnapshot();
        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        IEnumerable<CharacterSnapshot> characters = state.Entities.Values.OfType<CharacterSnapshot>().Where(c => c.PlayerId == player.UserId);

        return new TeamDto
        {
            Characters = characters.ToList()
                .Select(
                    c => c.ToDto(
                        new CharacterMappingOptions
                        {
                            OngoingAction = state.Actions.OngoingAction.GetValueOrDefault(c.Id),
                            PlannedAction = state.Actions.QueuedAction.GetValueOrDefault(c.Id)
                        }
                    )
                )
                .ToArray()
        };
    }
}
