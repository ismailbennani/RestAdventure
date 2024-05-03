using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using RestAdventure.Game.Authentication;
using RestAdventure.Game.Settings;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

[Route("game")]
[ApiController]
[Authorize(AuthenticationSchemes = GameApiAuthenticationOptions.AuthenticationScheme)]
[GameApi]
[OpenApiTag("Game")]
public class GameController : ControllerBase
{
    readonly IOptions<GameSettings> _gameSettings;

    public GameController(IOptions<GameSettings> gameSettings)
    {
        _gameSettings = gameSettings;
    }

    /// <summary>
    ///     Get settings
    /// </summary>
    /// <returns></returns>
    [HttpGet("settings")]
    public GameSettings GetGameSettings() => _gameSettings.Value;
}
