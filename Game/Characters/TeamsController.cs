using Game.Authentication;
using Game.Characters.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Characters;

/// <summary>
///     Team
/// </summary>
[Route("team")]
[ApiController]
[Authorize(AuthenticationSchemes = GameApiAuthenticationOptions.AuthenticationScheme)]
public class TeamsController : ControllerBase
{
    /// <summary>
    ///     Get team
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<TeamDto> GetTeam() => new() { Characters = Array.Empty<CharacterDto>() };
}
