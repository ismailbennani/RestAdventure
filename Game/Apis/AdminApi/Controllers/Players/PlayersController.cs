using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.AdminApi.Dtos.Players;

namespace RestAdventure.Game.Apis.AdminApi.Controllers.Players;

/// <summary>
///     Players operations
/// </summary>
[Route("admin/players")]
[OpenApiTag("Players")]
public class PlayersController : AdminApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public PlayersController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Get players
    /// </summary>
    [HttpGet]
    public ActionResult<IReadOnlyCollection<PlayerDto>> GetPlayers()
    {
        GameState state = _gameService.RequireGameState();
        return state.Players.All.Select(p => p.ToDto()).ToArray();
    }

    /// <summary>
    ///     Register player
    /// </summary>
    [HttpPost("{playerGuid:guid}")]
    public ActionResult<PlayerDto> RegisterPlayer(Guid playerGuid, string playerName)
    {
        PlayerId playerId = new(playerGuid);

        GameState state = _gameService.RequireGameState();
        Player player = state.Players.RegisterPlayer(playerId, playerName);
        return player.ToDto();
    }

    /// <summary>
    ///     Get player
    /// </summary>
    [HttpGet("{playerGuid:guid}")]
    public ActionResult<PlayerDto> GetPlayer(Guid playerGuid)
    {
        PlayerId playerId = new(playerGuid);

        GameState state = _gameService.RequireGameState();
        Player? player = state.Players.GetPlayer(playerId);
        if (player == null)
        {
            return Problem($"Could not find player {playerGuid}", statusCode: StatusCodes.Status400BadRequest);
        }

        return player.ToDto();
    }

    /// <summary>
    ///     Refresh player key
    /// </summary>
    [HttpPost("{playerGuid:guid}/refresh")]
    public ActionResult<PlayerDto> RefreshPlayerKey(Guid playerGuid)
    {
        PlayerId playerId = new(playerGuid);

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
