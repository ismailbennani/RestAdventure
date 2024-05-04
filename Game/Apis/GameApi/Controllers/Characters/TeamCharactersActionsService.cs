using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Core.Maps;
using RestAdventure.Game.Apis.GameApi.Services.Characters;
using RestAdventure.Game.Authentication;
using RestAdventure.Game.Controllers;
using RestAdventure.Kernel.Persistence;
using Xtensive.Orm;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

[Route("game/team/characters/{characterId:guid}")]
[OpenApiTag("Team")]
public class TeamCharactersActionsService : GameApiController
{
    readonly DomainAccessor _domainAccessor;
    readonly TeamService _teamService;
    readonly CharacterActionsService _characterActionsService;

    public TeamCharactersActionsService(DomainAccessor domainAccessor, TeamService teamService, CharacterActionsService characterActionsService)
    {
        _domainAccessor = domainAccessor;
        _teamService = teamService;
        _characterActionsService = characterActionsService;
    }

    /// <summary>
    ///     Move to location
    /// </summary>
    [HttpPost("move/{locationId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> MoveToLocationAsync(Guid characterId, Guid locationId)
    {
        await using Session session = await _domainAccessor.Domain.OpenSessionAsync();
        await using TransactionScope transaction = await session.OpenTransactionAsync();

        Guid playerId = ControllerContext.RequirePlayerId();

        OperationResult<CharacterDbo> character;
        using (session.Activate())
        {
            character = await _teamService.GetCharacterAsync(playerId, characterId);
        }

        if (!character.IsSuccess)
        {
            return ToFailedActionResult(character);
        }

        MapLocationDbo? location = await session.Query.All<MapLocationDbo>().SingleOrDefaultAsync(l => l.Id == locationId);

        using (session.Activate())
        {
            _characterActionsService.MoveToLocation(character.Result, location);
        }

        return NoContent();
    }
}
