﻿using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Game.Apis.GameApi.Dtos.Characters;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

[Route("game/team")]
[OpenApiTag("Team")]
public class TeamsController : GameApiController
{
    readonly GameService _gameService;
    readonly CharacterActionsService _characterActionsService;

    public TeamsController(GameService gameService, CharacterActionsService characterActionsService)
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
        Guid playerId = ControllerContext.RequirePlayerId();
        GameState state = _gameService.RequireGameState();

        Team team = state.Characters.GetTeamsOfPlayer(playerId).FirstOrDefault() ?? state.Characters.CreateTeam(playerId);

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
