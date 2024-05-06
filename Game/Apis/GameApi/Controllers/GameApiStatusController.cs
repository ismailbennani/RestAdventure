using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Status
/// </summary>
[Route("game/status")]
[OpenApiTag("Status")]
public class GameApiStatusController : GameApiController
{
    /// <summary>
    ///     Ping
    /// </summary>
    [HttpGet("ping")]
    public void Ping() { }
}
