using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Game.Apis.Common.Dtos;
using RestAdventure.Game.Services;

namespace RestAdventure.Game.Apis.AdminApi.Controllers;

/// <summary>
///     Game admin operations
/// </summary>
[Route("admin/game")]
[OpenApiTag("Game")]
public class AdminGameController : AdminApiController
{
    readonly GameService _gameService;
    readonly GameSimulation _gameSimulation;

    /// <summary>
    /// </summary>
    public AdminGameController(GameService gameService, GameSimulation gameSimulation)
    {
        _gameSimulation = gameSimulation;
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
        Core.Game state = _gameService.RequireGameState();
        return state.ToDto(_gameSimulation);
    }

    /// <summary>
    ///     Start simulation
    /// </summary>
    [HttpPost("start")]
    public void StartSimulation() => _gameSimulation.Start();

    /// <summary>
    ///     Tick now
    /// </summary>
    [HttpPost("tick")]
    public async Task TickNowAsync()
    {
        bool paused = _gameSimulation.Paused;

        if (!paused)
        {
            _gameSimulation.Stop();
        }

        await _gameSimulation.TickNowAsync();

        if (!paused)
        {
            _gameSimulation.Start(_gameSimulation.NextStepDate - DateTime.Now);
        }
    }

    /// <summary>
    ///     Stop simulation
    /// </summary>
    [HttpPost("stop")]
    public void StopSimulation() => _gameSimulation.Stop();
}
