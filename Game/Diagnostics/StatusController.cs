using Microsoft.AspNetCore.Mvc;

namespace RestAdventure.Game.Diagnostics;

/// <summary>
///     Status
/// </summary>
[Route("status")]
[ApiController]
public class StatusController : ControllerBase
{
    /// <summary>
    ///     Ping
    /// </summary>
    [HttpGet("ping")]
    public void Ping() { }
}
