using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.AdminApi.Controllers.Players.Requests;
using RestAdventure.Game.Apis.AdminApi.Dtos.Players;

namespace RestAdventure.Game.Apis.AdminApi.Controllers.Players;

[Route("admin/players")]
[ApiController]
[AdminApi]
[OpenApiTag("Players")]
public class PlayersController : ControllerBase
{
    readonly GameService _gameService;

    public PlayersController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Register player
    /// </summary>
    [HttpPost]
    public ActionResult<PlayerDto> RegisterPlayer(RegisterPlayerRequestDto request)
    {
        GameState state = _gameService.RequireGameState();
        Player player = state.Players.RegisterPlayer(request.PlayerId, request.PlayerName);
        return player.ToDto();
    }

    /// <summary>
    ///     Get player
    /// </summary>
    [HttpGet]
    public ActionResult<PlayerDto> GetPlayer(Guid playerId)
    {
        GameState state = _gameService.RequireGameState();
        Player? player = state.Players.GetPlayer(playerId);
        if (player == null)
        {
            return Problem($"Could not find player {playerId}", statusCode: StatusCodes.Status400BadRequest);
        }

        return player.ToDto();
    }

    /// <summary>
    ///     Refresh player key
    /// </summary>
    [HttpPost("refresh")]
    public ActionResult<PlayerDto> RefreshPlayerKey(Guid playerId)
    {
        GameState state = _gameService.RequireGameState();
        Player? player = state.Players.GetPlayer(playerId);
        if (player == null)
        {
            return Problem($"Could not find player {playerId}", statusCode: StatusCodes.Status400BadRequest);
        }

        player.RefreshApiKey();

        return player.ToDto();
    }
}
