using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core.Characters;
using RestAdventure.Game.Apis.GameApi.Controllers.Characters.Requests;
using RestAdventure.Game.Apis.GameApi.Dtos.Characters;
using RestAdventure.Game.Apis.GameApi.Services.Characters;
using RestAdventure.Game.Authentication;
using RestAdventure.Game.Controllers;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

[Route("game/team/characters")]
[OpenApiTag("Team")]
public class TeamCharactersController : GameApiController
{
    readonly DomainAccessor _domainAccessor;
    readonly TeamService _teamService;

    public TeamCharactersController(DomainAccessor domainAccessor, TeamService teamService)
    {
        _domainAccessor = domainAccessor;
        _teamService = teamService;
    }

    /// <summary>
    ///     Create character
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TeamCharacterDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamCharacterDto>> CreateCharacterAsync(CreateCharacterRequestDto request)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        Guid playerId = ControllerContext.RequirePlayerId();

        OperationResult<CharacterDbo> result;
        using (session.Activate())
        {
            result = await _teamService.CreateCharacterAsync(playerId, request);
        }

        if (!result.IsSuccess)
        {
            return Problem($"Could not create character: {result.ErrorMessage}", statusCode: StatusCodes.Status400BadRequest);
        }

        transaction.Complete();

        return ToActionResult(result, t => t.ToDto());
    }

    /// <summary>
    ///     Delete character
    /// </summary>
    [HttpDelete("{characterId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCharacterAsync(Guid characterId)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        Guid playerId = ControllerContext.RequirePlayerId();

        OperationResult result;
        using (session.Activate())
        {
            result = await _teamService.DeleteCharacterServiceAsync(playerId, characterId);
        }

        transaction.Complete();

        return ToActionResult(result);
    }
}
