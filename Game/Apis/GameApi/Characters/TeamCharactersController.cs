using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Game.Apis.GameApi.Characters.Dtos;
using RestAdventure.Game.Apis.GameApi.Characters.Requests;
using RestAdventure.Game.Apis.GameApi.Characters.Services;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Apis.GameApi.Characters;

[Route("game/team/characters")]
[ApiController]
[Authorize(AuthenticationSchemes = GameApiAuthenticationOptions.AuthenticationScheme)]
[GameApi]
[OpenApiTag("Team")]
public class TeamCharactersController : ControllerBase
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
    public async Task<ActionResult<CharacterDto>> CreateCharacterAsync(CreateCharacterRequestDto request)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        Guid playerId = ControllerContext.RequirePlayerId();

        CharacterCreationResult result;
        using (session.Activate())
        {
            result = await _teamService.CreateCharacterAsync(playerId, request);
        }

        if (!result.IsSuccess)
        {
            return Problem($"Could not create character: {result.ErrorMessage}", statusCode: StatusCodes.Status400BadRequest);
        }

        transaction.Complete();

        return result.Character.ToDto();
    }

    /// <summary>
    ///     Delete character
    /// </summary>
    [HttpDelete("{characterId:guid}")]
    public async Task<IActionResult> DeleteCharacterAsync(Guid characterId)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        Guid playerId = ControllerContext.RequirePlayerId();

        bool success;
        using (session.Activate())
        {
            success = await _teamService.DeleteCharacterServiceAsync(playerId, characterId);
        }

        if (!success)
        {
            return Problem($"Could not delete character {characterId}", statusCode: StatusCodes.Status404NotFound);
        }

        transaction.Complete();

        return NoContent();
    }
}
