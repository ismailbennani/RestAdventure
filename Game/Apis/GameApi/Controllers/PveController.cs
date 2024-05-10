using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Combats;
using RestAdventure.Game.Apis.Common.Dtos.Monsters;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     PVE operations
/// </summary>
[Route("game/team/characters/{characterGuid:guid}/pve")]
[OpenApiTag("Pve")]
public class PveController : GameApiController
{
    readonly GameService _gameService;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// </summary>
    public PveController(GameService gameService, ILoggerFactory loggerFactory)
    {
        _gameService = gameService;
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    ///     Get monsters
    /// </summary>
    [HttpGet("monsters")]
    [ProducesResponseType<IReadOnlyCollection<MonsterGroupDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyCollection<MonsterGroupDto>> GetMonsters(Guid characterGuid)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IEnumerable<PveCombatAction> actions = state.Actions.GetAvailableActions(character).OfType<PveCombatAction>();

        List<MonsterGroupDto> result = [];
        foreach (PveCombatAction action in actions)
        {
            Maybe canStartCombat = action.CanPerform(state, character);
            result.Add(
                new MonsterGroupDto
                {
                    Id = action.MonsterGroup!.Id.Guid,
                    Monsters = action.MonsterGroup.Monsters.Select(m => m.ToDto()).ToArray(),
                    ExpectedExperience = action.MonsterGroup.Monsters.Sum(m => m.Species.Experience),
                    CanAttack = canStartCombat.Success,
                    WhyCannotAttack = canStartCombat.WhyNot
                }
            );
        }

        return result;
    }

    /// <summary>
    ///     Attack monsters
    /// </summary>
    [HttpPost("monsters/{groupId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult AttackMonsters(Guid characterGuid, Guid groupId, CombatFormationOptionsDto? options = null)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        MonsterGroupId monsterGroupId = new(groupId);
        PveCombatAction? action = state.Actions.GetAvailableActions(character).OfType<PveCombatAction>().SingleOrDefault(a => a.MonsterGroup.Id == monsterGroupId);
        if (action == null)
        {
            return NotFound();
        }

        if (options != null)
        {
            action.SetOptions(options.ToBusiness());
        }

        state.Actions.QueueAction(character, action);

        return NoContent();
    }

    /// <summary>
    ///     Join combat in preparation
    /// </summary>
    [HttpPost("{combatGuid:guid}/join")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult JoinCombatInPreparation(Guid characterGuid, Guid combatGuid)
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
        PveCombatAction? action = state.Actions.GetAvailableActions(character)
            .OfType<PveCombatAction>()
            .SingleOrDefault(a => a.CombatInPreparation != null && a.CombatInPreparation.Id == combatId);

        if (action == null || action.CombatInPreparation!.Location != character.Location)
        {
            return NotFound();
        }

        state.Actions.QueueAction(character, action);

        return NoContent();
    }
}
