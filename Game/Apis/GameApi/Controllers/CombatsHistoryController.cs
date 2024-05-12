using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.History.Combats;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Combats;
using RestAdventure.Game.Apis.Common.Dtos.History.Combats;
using RestAdventure.Game.Apis.Common.Dtos.Queries;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Queries;
using CombatInstance = RestAdventure.Core.Combat.CombatInstance;

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
        Core.Game state = _gameService.RequireGameState();

        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);
        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        CombatInstanceId combatId = new(combatGuid);
        CombatInstance? combat = state.Combats.GetCombat(combatId);
        if (combat == null || combat.Location != character.Location)
        {
            return NotFound();
        }

        IOrderedEnumerable<CombatHistoryEntry> allEntries = state.History.Combat(combatId).OrderByDescending(he => he.Tick);

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
        Core.Game state = _gameService.RequireGameState();

        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);
        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IEnumerable<CombatEndedHistoryEntry> archivedCombats = state.History.Combat(character.Id).OfType<CombatEndedHistoryEntry>().OrderByDescending(he => he.Tick);

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
        Core.Game state = _gameService.RequireGameState();

        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);
        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        CombatInstanceId combatId = new(combatGuid);
        CombatEndedHistoryEntry? archivedCombat = state.History.Combat(character.Id).OfType<CombatEndedHistoryEntry>().SingleOrDefault(c => c.CombatInstanceId == combatId);
        if (archivedCombat == null)
        {
            return NotFound();
        }

        IOrderedEnumerable<CombatHistoryEntry> history = state.History.Combat(combatId).OrderByDescending(he => he.Tick);
        return Search.Paginate(history, request.ToPaginationParameters()).ToDto(c => c.ToDto());
    }
}
