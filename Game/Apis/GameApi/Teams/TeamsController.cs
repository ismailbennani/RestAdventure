using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAdventure.Game.Apis.GameApi.Teams.Dtos;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Teams;

/// <summary>
///     Team
/// </summary>
[Route("game/team")]
[ApiController]
[Authorize(AuthenticationSchemes = GameApiAuthenticationOptions.AuthenticationScheme)]
[GameApi]
public class TeamsController : ControllerBase
{
    /// <summary>
    ///     Get team
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<TeamDto> GetTeam() => new() { Characters = Array.Empty<CharacterDto>() };
}
