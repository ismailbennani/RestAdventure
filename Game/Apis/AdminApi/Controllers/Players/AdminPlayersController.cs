using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Players;
using RestAdventure.Kernel.Security;

namespace RestAdventure.Game.Apis.AdminApi.Controllers.Players;

/// <summary>
///     Players operations
/// </summary>
[Route("admin/players")]
[OpenApiTag("Players")]
public class AdminPlayersController : AdminApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public AdminPlayersController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Get players
    /// </summary>
    [HttpGet]
    public ActionResult<IReadOnlyCollection<PlayerDto>> GetPlayers()
    {
        Core.Game state = _gameService.RequireGame();
        return state.Players.All.Select(p => p.ToDto()).ToArray();
    }

    /// <summary>
    ///     Register player
    /// </summary>
    [HttpPost("{userGuid:guid}")]
    public async Task<ActionResult<PlayerDto>> RegisterPlayerAsync(Guid userGuid, string playerName)
    {
        UserId userId = new(userGuid);

        Core.Game state = _gameService.RequireGame();

        Player? existingPlayer = state.Players.GetPlayer(userId);
        if (existingPlayer != null)
        {
            return existingPlayer.ToDto();
        }

        User user = new(userId, playerName);
        Player player = await state.Players.RegisterPlayerAsync(user);

        return player.ToDto();
    }

    /// <summary>
    ///     Get player
    /// </summary>
    [HttpGet("{userGuid:guid}")]
    public ActionResult<PlayerDto> GetPlayer(Guid userGuid)
    {
        UserId userId = new(userGuid);

        Core.Game state = _gameService.RequireGame();
        Player? player = state.Players.GetPlayer(userId);
        if (player == null)
        {
            return Problem($"Could not find player for {userGuid}", statusCode: StatusCodes.Status400BadRequest);
        }

        return player.ToDto();
    }

    /// <summary>
    ///     Refresh player key
    /// </summary>
    [HttpPost("{userGuid:guid}/refresh")]
    public ActionResult<PlayerDto> RefreshPlayerKey(Guid userGuid)
    {
        UserId userId = new(userGuid);

        Core.Game state = _gameService.RequireGame();
        Player? player = state.Players.GetPlayer(userId);
        if (player == null)
        {
            return Problem($"Could not find player for {userGuid}", statusCode: StatusCodes.Status400BadRequest);
        }

        player.User.RefreshApiKey();

        return player.ToDto();
    }
}
