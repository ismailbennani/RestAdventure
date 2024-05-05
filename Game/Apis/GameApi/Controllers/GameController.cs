using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Settings;
using RestAdventure.Game.Apis.GameApi.Dtos;
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
    readonly IOptions<GameSettings> _gameSettings;

    /// <summary>
    /// </summary>
    public GameController(GameService gameService, GameScheduler gameScheduler, IOptions<GameSettings> gameSettings)
    {
        _gameSettings = gameSettings;
        _gameService = gameService;
        _gameScheduler = gameScheduler;
    }

    /// <summary>
    ///     Get game settings
    /// </summary>
    /// <returns></returns>
    [HttpGet("settings")]
    public GameSettings GetGameSettings() => _gameSettings.Value;

    /// <summary>
    ///     Get game state
    /// </summary>
    /// <returns></returns>
    [HttpGet("state")]
    public GameStateDto GetGameState()
    {
        GameState state = _gameService.RequireGameState();

        return new GameStateDto
        {
            Tick = state.Tick,
            Paused = _gameScheduler.Paused,
            LastTickDate = _gameScheduler.LastStepDate,
            NextTickDate = _gameScheduler.NextStepDate
        };
    }
}
