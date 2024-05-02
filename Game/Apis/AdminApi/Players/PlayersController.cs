using Microsoft.AspNetCore.Mvc;
using RestAdventure.Game.Registration;

namespace RestAdventure.Game.Apis.AdminApi.Players;

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
    ///     Register player
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PlayerRegistrationDto>> RegisterPlayer(Guid playerId)
    {
        PlayerRegistrationDto? registration = await _playerRegistrationService.RegisterPlayer(playerId);
        if (registration == null)
        {
            return BadRequest($"Could not register player {playerId}");
        }

        return registration;
    }

    /// <summary>
    ///     Get registration
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PlayerRegistrationDto>> GetApiKey(Guid playerId)
    {
        PlayerRegistrationDto? registration = await _playerRegistrationService.GetRegistration(playerId);
        if (registration == null)
        {
            return BadRequest($"Could not find registration of player {playerId}");
        }

        return registration;
    }

    /// <summary>
    ///     Refresh registration
    /// </summary>
    /// <returns></returns>
    [HttpPost("refresh")]
    public async Task<ActionResult<PlayerRegistrationDto>> RefreshApiKey(Guid playerId)
    {
        PlayerRegistrationDto? newRegistration = await _playerRegistrationService.RefreshApiKey(playerId);
        if (newRegistration == null)
        {
            return BadRequest($"Could not refresh registration of player {playerId}");
        }

        return newRegistration;
    }
}
