using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.History.Combats;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Combats;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Combats;
using RestAdventure.Game.Apis.Common.Dtos.History.Combats;
using RestAdventure.Game.Apis.Common.Dtos.Queries;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Queries;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Combats history operations
/// </summary>
[Route("game/team/characters/{characterGuid:guid}/combats")]
[OpenApiTag("Combats")]
public class CombatsHistoryController : GameApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public CombatsHistoryController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Search combat history
    /// </summary>
    [HttpGet("{combatGuid:guid}/history")]
    [ProducesResponseType<SearchResultDto<CombatHistoryEntryDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<SearchResultDto<CombatHistoryEntryDto>> SearchCombatHistory(Guid characterGuid, Guid combatGuid, [FromQuery] SearchRequestDto request)
    {
        GameSnapshot state = _gameService.GetLastSnapshot();

        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        CombatInstanceId combatId = new(combatGuid);
        CombatInstanceSnapshot? combat = state.Combats.GetValueOrDefault(combatId);
        if (combat == null || combat.Location != character.Location)
        {
            return NotFound();
        }

        IOrderedEnumerable<CombatHistoryEntry> allEntries = GetCombatHistory(state, combat.Id);

        return Search.Paginate(allEntries, request.ToPaginationParameters()).ToDto(c => c.ToDto());
    }

    /// <summary>
    ///     Search archived combats
    /// </summary>
    [HttpGet("archived")]
    [ProducesResponseType<SearchResultDto<ArchivedCombatDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<SearchResultDto<ArchivedCombatDto>> SearchArchivedCombats(Guid characterGuid, [FromQuery] SearchRequestDto request)
    {
        GameSnapshot state = _gameService.GetLastSnapshot();

        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        IEnumerable<CombatEndedHistoryEntry> archivedCombats = GetArchivedCombatsOfCharacter(state, characterId);

        return Search.Paginate(archivedCombats, request.ToPaginationParameters()).ToDto(c => c.ToArchivedCombatDto());
    }

    /// <summary>
    ///     Search archived combats history
    /// </summary>
    [HttpGet("archived/{combatGuid:guid}/history")]
    [ProducesResponseType<SearchResultDto<CombatHistoryEntryDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<SearchResultDto<CombatHistoryEntryDto>> SearchArchivedCombatHistory(Guid characterGuid, Guid combatGuid, [FromQuery] SearchRequestDto request)
    {
        GameSnapshot state = _gameService.GetLastSnapshot();

        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        CombatInstanceId combatId = new(combatGuid);
        CombatEndedHistoryEntry? archivedCombat = GetArchivedCombatsOfCharacter(state, character.Id).SingleOrDefault(c => c.CombatInstanceId == combatId);
        if (archivedCombat == null)
        {
            return NotFound();
        }

        IOrderedEnumerable<CombatHistoryEntry> history = GetCombatHistory(state, archivedCombat.CombatInstanceId);
        return Search.Paginate(history, request.ToPaginationParameters()).ToDto(c => c.ToDto());
    }

    static IOrderedEnumerable<CombatHistoryEntry> GetCombatHistory(GameSnapshot state, CombatInstanceId combatId)
    {
        IOrderedEnumerable<CombatHistoryEntry> allEntries = state.History.OfType<CombatHistoryEntry>().Where(c => c.CombatInstanceId == combatId).OrderByDescending(he => he.Tick);
        return allEntries;
    }

    static IEnumerable<CombatEndedHistoryEntry> GetArchivedCombatsOfCharacter(GameSnapshot state, CharacterId characterId)
    {
        IEnumerable<CombatEndedHistoryEntry> archivedCombats = state.History.OfType<CombatEndedHistoryEntry>()
            .Where(c => c.Attackers.Any(a => a.Id == characterId) || c.Defenders.Any(a => a.Id == characterId))
            .OrderByDescending(he => he.Tick);
        return archivedCombats;
    }
}
