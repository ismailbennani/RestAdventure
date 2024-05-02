﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAdventure.Game.Characters.Dtos;

namespace RestAdventure.Game.Characters;

/// <summary>
///     Team
/// </summary>
[Route("team")]
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
