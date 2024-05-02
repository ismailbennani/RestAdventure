using Microsoft.AspNetCore.Mvc;
using RestAdventure.Game.Registration;

namespace RestAdventure.Game.Apis.AdminApi;

[Route("admin/players")]
[ApiController]
[AdminApi]
public class PlayersController : ControllerBase
{
    readonly PlayerRegistrationService _playerRegistrationService;

    public PlayersController(PlayerRegistrationService playerRegistrationService)
    {
        _playerRegistrationService = playerRegistrationService;
    }

    /// <summary>
    ///     Get team
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PlayerRegistrationDto>> RegisterPlayer(Guid playerId)
    {
        PlayerRegistrationDto? registration = await _playerRegistrationService.RegisterPlayer(playerId);
        if (registration == null)
        {
            return BadRequest($"Could not register player {playerId}");
        }

        return registration;
    }
}
