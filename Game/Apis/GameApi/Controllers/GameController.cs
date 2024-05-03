using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Game.Apis.GameApi.Dtos;
using RestAdventure.Game.Apis.GameApi.Services.Game;
using RestAdventure.Game.Authentication;
using RestAdventure.Game.Settings;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

[Route("game")]
[ApiController]
[Authorize(AuthenticationSchemes = GameApiAuthenticationOptions.AuthenticationScheme)]
[GameApi]
[OpenApiTag("Game")]
public class GameController : ControllerBase
{
    readonly DomainAccessor _domainAccessor;
    readonly GameService _gameService;
    readonly GameScheduler _gameScheduler;
    readonly IOptions<GameSettings> _gameSettings;

    public GameController(DomainAccessor domainAccessor, GameService gameService, GameScheduler gameScheduler, IOptions<GameSettings> gameSettings)
    {
        _gameSettings = gameSettings;
        _gameService = gameService;
        _gameScheduler = gameScheduler;
        _domainAccessor = domainAccessor;
    }

    /// <summary>
    ///     Get game settings
    /// </summary>
    /// <returns></returns>
    [HttpGet("settings")]
    public GameSettings GetGameSettings() => _gameSettings.Value;

    /// <summary>
    ///     Get game state
    /// </summary>
    /// <returns></returns>
    [HttpGet("state")]
    public async Task<GameStateDto> GetGameStateAsync()
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        GameStateDbo state;
        using (session.Activate())
        {
            state = await _gameService.GetStateAsync();
        }

        return new GameStateDto
        {
            Tick = state.Tick,
            Paused = _gameScheduler.Paused,
            LastTickDate = _gameScheduler.LastStepDate,
            NextTickDate = _gameScheduler.NextStepDate
        };
    }
}
