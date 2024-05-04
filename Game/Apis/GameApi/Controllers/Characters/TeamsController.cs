using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Game.Apis.GameApi.Dtos.Characters;
using RestAdventure.Game.Apis.GameApi.Services.Characters;
using RestAdventure.Game.Authentication;
using RestAdventure.Game.Controllers;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

[Route("game/team")]
[OpenApiTag("Team")]
public class TeamsController : GameApiController
{
    readonly DomainAccessor _domainAccessor;
    readonly TeamService _teamService;
    readonly CharacterActionsService _characterActionsService;

    public TeamsController(DomainAccessor domainAccessor, TeamService teamService, CharacterActionsService characterActionsService)
    {
        _domainAccessor = domainAccessor;
        _teamService = teamService;
        _characterActionsService = characterActionsService;
    }

    /// <summary>
    ///     Get team
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<TeamDto>> GetTeamAsync()
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

        transaction.Complete();

        IEnumerable<TeamCharacterDto> characterDtos;
        using (session.Activate())
        {
            characterDtos = team.Result.Characters.ToList()
                .Select(
                    c => c.ToDto(
                        new CharacterMappingOptions
                        {
                            LastActionResult = _characterActionsService.GetLastActionResult(c),
                            NextAction = _characterActionsService.GetNextAction(c)
                        }
                    )
                );
        }

        return ToActionResult(team, t => new TeamDto { Characters = characterDtos.ToArray() });
    }

    static async Task<TeamDbo> GetOrCreateTeamAsync(Session session, Guid playerId)
    {
        using SessionScope _ = session.Activate();
        return await session.Query.All<TeamDbo>().SingleOrDefaultAsync(t => t.PlayerId == playerId) ?? new TeamDbo(playerId);
    }
}
