using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAdventure.Game.GameApi.Teams.Dtos;

namespace RestAdventure.Game.GameApi.Teams;

/// <summary>
///     Team
/// </summary>
[Route("game/team")]
[ApiController]
[Authorize]
public class TeamsController : ControllerBase
{
    /// <summary>
    ///     Get team
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<TeamDto> GetTeam() => new() { Characters = Array.Empty<CharacterDto>() };
}
