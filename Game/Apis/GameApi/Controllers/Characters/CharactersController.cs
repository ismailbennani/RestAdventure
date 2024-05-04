using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core.Characters;
using RestAdventure.Game.Apis.GameApi.Dtos.Characters;
using RestAdventure.Game.Apis.GameApi.Services.Characters;
using RestAdventure.Game.Authentication;
using RestAdventure.Game.Controllers;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

[Route("game/characters")]
[OpenApiTag("Characters")]
public class CharactersController : GameApiController
{
    readonly DomainAccessor _domainAccessor;
    readonly TeamService _teamService;
    readonly CharacterInteractionsService _characterInteractionsService;

    public CharactersController(DomainAccessor domainAccessor, TeamService teamService, CharacterInteractionsService characterInteractionsService)
    {
        _domainAccessor = domainAccessor;
        _teamService = teamService;
        _characterInteractionsService = characterInteractionsService;
    }

    /// <summary>
    ///     Get characters in range
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CharacterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<CharacterDto>>> GetCharactersInRangeAsync()
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        Guid playerId = ControllerContext.RequirePlayerId();

        OperationResult<TeamDbo> team;
        using (session.Activate())
        {
            team = await _teamService.GetTeamAsync(playerId);
        }

        if (!team.IsSuccess)
        {
            return ToFailedActionResult(team);
        }

        List<CharacterDbo> result = await _characterInteractionsService.GetCharactersInRange(team.Result).ToListAsync();

        return result.Select(c => c.ToOtherCharacterDto()).ToArray();
    }
}
