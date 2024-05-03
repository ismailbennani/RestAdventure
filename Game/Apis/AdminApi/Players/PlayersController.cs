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
            return Problem($"Could not register player {playerId}", statusCode: StatusCodes.Status400BadRequest);
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
            return Problem($"Could not find registration of player {playerId}", statusCode: StatusCodes.Status400BadRequest);
        }

        return registration;
    }

    /// <summary>
    ///     Refresh registration
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<PlayerRegistrationDto>> RefreshApiKey(Guid playerId)
    {
        PlayerRegistrationDto? newRegistration = await _playerRegistrationService.RefreshApiKey(playerId);
        if (newRegistration == null)
        {
            return Problem($"Could not refresh registration of player {playerId}", statusCode: StatusCodes.Status400BadRequest);
        }

        return newRegistration;
    }
}
