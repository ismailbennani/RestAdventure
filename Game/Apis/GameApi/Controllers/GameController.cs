using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Game.Apis.Common.Dtos;
using RestAdventure.Game.Apis.GameApi.Services.Game;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Game operations
/// </summary>
[Route("game")]
[OpenApiTag("Game")]
public class GameController : GameApiController
{
    readonly GameService _gameService;
    readonly GameScheduler _gameScheduler;

    /// <summary>
    /// </summary>
    public GameController(GameService gameService, GameScheduler gameScheduler)
    {
        _gameService = gameService;
        _gameScheduler = gameScheduler;
    }

    /// <summary>
    ///     Get game settings
    /// </summary>
    [HttpGet("settings")]
    public GameSettingsDto GetGameSettings() => _gameService.RequireGameState().Settings.ToDto();

    /// <summary>
    ///     Get game state
    /// </summary>
    [HttpGet("state")]
    public GameStateDto GetGameState()
    {
        GameState state = _gameService.RequireGameState();

        return new GameStateDto
        {
            Tick = state.Tick,
            Paused = _gameScheduler.Paused,
            LastTickDate = _gameScheduler.LastStepDate,
            NextTickDate = _gameScheduler.Paused ? null : _gameScheduler.NextStepDate
        };
    }
}
