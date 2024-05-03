using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core.Characters;
using RestAdventure.Game.Apis.GameApi.Dtos.Characters;
using RestAdventure.Game.Apis.GameApi.Services.Characters;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

[Route("game/characters")]
[ApiController]
[Authorize(AuthenticationSchemes = GameApiAuthenticationOptions.AuthenticationScheme)]
[GameApi]
[OpenApiTag("Characters")]
public class CharactersController : ControllerBase
{
    readonly DomainAccessor _domainAccessor;
    readonly TeamService _teamService;
    readonly CharactersInteractionService _charactersInteractionService;

    public CharactersController(DomainAccessor domainAccessor, TeamService teamService, CharactersInteractionService charactersInteractionService)
    {
        _domainAccessor = domainAccessor;
        _teamService = teamService;
        _charactersInteractionService = charactersInteractionService;
    }

    /// <summary>
    ///     Get characters in range
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IReadOnlyCollection<CharacterDto>> GetCharactersInRangeAsync()
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        Guid playerId = ControllerContext.RequirePlayerId();

        TeamDbo team;
        using (session.Activate())
        {
            team = await _teamService.GetTeamAsync(playerId);
        }

        List<CharacterDbo> result = await _charactersInteractionService.GetCharactersInRange(team).ToListAsync();

        return result.Select(c => c.ToOtherCharacterDto()).ToArray();
    }
}
