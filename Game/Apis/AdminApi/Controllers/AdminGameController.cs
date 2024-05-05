using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Game.Apis.GameApi.Services.Game;

namespace RestAdventure.Game.Apis.AdminApi.Controllers;

/// <summary>
///     Game admin operations
/// </summary>
[Route("admin/game")]
[OpenApiTag("Game")]
public class AdminGameController : AdminApiController
{
    readonly GameScheduler _gameScheduler;

    /// <summary>
    /// </summary>
    public AdminGameController(GameScheduler gameScheduler)
    {
        _gameScheduler = gameScheduler;
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
