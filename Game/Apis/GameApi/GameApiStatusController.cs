using Microsoft.AspNetCore.Mvc;

namespace RestAdventure.Game.Apis.GameApi;

/// <summary>
///     Status
/// </summary>
[Route("game/status")]
public class GameApiStatusController : GameApiController
{
    /// <summary>
    ///     Ping
    /// </summary>
    [HttpGet("ping")]
    public void Ping() { }
}
