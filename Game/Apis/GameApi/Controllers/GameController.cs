using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Serialization;
using RestAdventure.Game.Apis.Common.Dtos;
using RestAdventure.Game.Services;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Game operations
/// </summary>
[Route("game")]
[OpenApiTag("Game")]
public class GameController : GameApiController
{
    readonly GameService _gameService;
    readonly GameSimulation _gameSimulation;

    /// <summary>
    /// </summary>
    public GameController(GameService gameService, GameSimulation gameSimulation)
    {
        _gameService = gameService;
        _gameSimulation = gameSimulation;
    }

    /// <summary>
    ///     Get game settings
    /// </summary>
    [HttpGet("settings")]
    public GameSettingsDto GetGameSettings() => _gameService.GetLastSnapshot().Settings.ToDto();

    /// <summary>
    ///     Get game state
    /// </summary>
    [HttpGet("state")]
    public GameStateDto GetGameState()
    {
        GameSnapshot state = _gameService.GetLastSnapshot();
        return state.ToDto(_gameSimulation);
    }
}
