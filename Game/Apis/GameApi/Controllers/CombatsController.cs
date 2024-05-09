using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Combats;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Combats operations
/// </summary>
[Route("game/team/characters/{characterGuid:guid}/combats")]
[OpenApiTag("Combats")]
public class CombatsController : GameApiController
{
    readonly GameService _gameService;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// </summary>
    public CombatsController(GameService gameService, ILoggerFactory loggerFactory)
    {
        _gameService = gameService;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    ///     Get combats in preparation
    /// </summary>
    [HttpGet("in-preparation")]
    [ProducesResponseType<IReadOnlyCollection<CombatInPreparationDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyCollection<CombatInPreparationDto>> GetCombatsInPreparation(Guid characterGuid)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IEnumerable<CombatInPreparation> combats = state.Combats.GetCombatInPreparationAtLocation(character.Location);
        return combats.Select(c => c.ToDto(c.Attackers.CanJoin(character))).ToArray();
    }

    /// <summary>
    ///     Join combat in preparation
    /// </summary>
    [HttpPost("in-preparation/{combatGuid:guid}/{side}/join")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult JoinCombatInPreparation(Guid characterGuid, Guid combatGuid, CombatSide side)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        CombatInstanceId combatId = new(combatGuid);
        CombatInPreparation? combatInPreparation = state.Combats.GetCombatInPreparation(combatId);
        if (combatInPreparation == null || combatInPreparation.Location != character.Location)
        {
            return NotFound();
        }

        PveCombatAction action = new(combatInPreparation);
        state.Actions.QueueAction(character, action);

        return NoContent();
    }

    /// <summary>
    ///     Set combat in preparation options
    /// </summary>
    [HttpPost("in-preparation/{combatGuid:guid}/{side}/options")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult SetCombatInPreparationOptions(Guid characterGuid, Guid combatGuid, CombatSide side, CombatFormationOptionsDto options)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        CombatInstanceId combatId = new(combatGuid);
        CombatInPreparation? combatInPreparation = state.Combats.GetCombatInPreparation(combatId);
        if (combatInPreparation == null || combatInPreparation.Location != character.Location)
        {
            return NotFound();
        }

        Maybe performed = combatInPreparation.GetTeam(side).SetOptions(character, options.ToBusiness());
        if (!performed.Success)
        {
            return Problem(performed.WhyNot, statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }

    /// <summary>
    ///     Get combats
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<CombatInstanceDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyCollection<CombatInstanceDto>> GetCombats(Guid characterGuid)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IEnumerable<CombatInstance> combats = state.Combats.GetCombatAtLocation(character.Location);
        return combats.Select(c => c.ToDto()).ToArray();
    }
}
