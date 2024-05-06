using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Game.Apis.Common.Dtos;
using RestAdventure.Game.Apis.GameApi.Services.Game;

namespace RestAdventure.Game.Apis.AdminApi.Controllers;

/// <summary>
///     Game admin operations
/// </summary>
[Route("admin/game")]
[OpenApiTag("Game")]
public class AdminGameController : AdminApiController
{
    readonly GameService _gameService;
    readonly GameScheduler _gameScheduler;

    /// <summary>
    /// </summary>
    public AdminGameController(GameService gameService, GameScheduler gameScheduler)
    {
        _gameScheduler = gameScheduler;
        _gameService = gameService;
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

    /// <summary>
    ///     Start simulation
    /// </summary>
    [HttpPost("start")]
    public void StartSimulation() => _gameScheduler.Start();

    /// <summary>
    ///     Tick now
    /// </summary>
    [HttpPost("tick")]
    public async Task TickNowAsync()
    {
        bool paused = _gameScheduler.Paused;

        if (!paused)
        {
            _gameScheduler.Stop();
        }

        await _gameScheduler.TickNowAsync();

        if (!paused)
        {
            _gameScheduler.Start(_gameScheduler.NextStepDate - DateTime.Now);
        }
    }

    /// <summary>
    ///     Stop simulation
    /// </summary>
    [HttpPost("stop")]
    public void StopSimulation() => _gameScheduler.Stop();
}
