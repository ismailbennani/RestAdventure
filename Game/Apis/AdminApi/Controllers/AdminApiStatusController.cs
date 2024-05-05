using Microsoft.AspNetCore.Mvc;

namespace RestAdventure.Game.Apis.AdminApi.Controllers;

/// <summary>
///     Status
/// </summary>
[Route("admin/status")]
public class AdminApiStatusController : AdminApiController
{
    /// <summary>
    ///     Ping
    /// </summary>
    [HttpGet("ping")]
    public void Ping() { }
}
