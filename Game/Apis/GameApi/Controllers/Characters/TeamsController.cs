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

[Route("game/team")]
[ApiController]
[Authorize(AuthenticationSchemes = GameApiAuthenticationOptions.AuthenticationScheme)]
[GameApi]
[OpenApiTag("Team")]
public class TeamsController : ControllerBase
{
    readonly DomainAccessor _domainAccessor;
    readonly TeamService _teamService;

    public TeamsController(DomainAccessor domainAccessor, TeamService teamService)
    {
        _domainAccessor = domainAccessor;
        _teamService = teamService;
    }

    /// <summary>
    ///     Get team
    /// </summary>
    [HttpGet]
    public async Task<TeamDto> GetTeamAsync()
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        Guid playerId = ControllerContext.RequirePlayerId();

        TeamDbo team;
        using (session.Activate())
        {
            team = await _teamService.GetTeamAsync(playerId);
        }

        transaction.Complete();

        return team.ToDto();
    }

    static async Task<TeamDbo> GetOrCreateTeamAsync(Session session, Guid playerId)
    {
        using SessionScope _ = session.Activate();
        return await session.Query.All<TeamDbo>().SingleOrDefaultAsync(t => t.PlayerId == playerId) ?? new TeamDbo(playerId);
    }
}
