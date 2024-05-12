using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Monsters;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

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
        Core.Game state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IReadOnlyCollection<Action> actions = state.Actions.GetAvailableActions(character);
        IEnumerable<StartAndPlayPveCombatAction> startCombatActions = actions.OfType<StartAndPlayPveCombatAction>();
        IEnumerable<JoinAndPlayPveCombatAction> joinCombatActions = actions.OfType<JoinAndPlayPveCombatAction>();

        List<MonsterGroupDto> result = [];

        foreach (JoinAndPlayPveCombatAction action in joinCombatActions)
        {
            Maybe canStartCombat = action.CanPerform(state, character);
            result.Add(
                new MonsterGroupDto
                {
                    Id = action.MonsterGroup.Id.Guid,
                    Monsters = action.MonsterGroup.Monsters.Select(m => m.ToDto()).ToArray(),
                    ExpectedExperience = action.MonsterGroup.Monsters.Sum(m => m.Species.Experience),
                    Attacked = true,
                    CanAttackOrJoin = canStartCombat.Success,
                    WhyCannotAttackOrJoin = canStartCombat.WhyNot
                }
            );
        }

        foreach (StartAndPlayPveCombatAction action in startCombatActions)
        {
            Maybe canStartCombat = action.CanPerform(state, character);
            result.Add(
                new MonsterGroupDto
                {
                    Id = action.MonsterGroup.Id.Guid,
                    Monsters = action.MonsterGroup.Monsters.Select(m => m.ToDto()).ToArray(),
                    ExpectedExperience = action.MonsterGroup.Monsters.Sum(m => m.Species.Experience),
                    Attacked = false,
                    CanAttackOrJoin = canStartCombat.Success,
                    WhyCannotAttackOrJoin = canStartCombat.WhyNot
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
    public ActionResult AttackMonsters(Guid characterGuid, Guid groupId)
    {
        Core.Game state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        MonsterGroupId monsterGroupId = new(groupId);
        StartAndPlayPveCombatAction? action = state.Actions.GetAvailableActions(character)
            .OfType<StartAndPlayPveCombatAction>()
            .SingleOrDefault(a => a.MonsterGroup.Id == monsterGroupId);
        if (action == null)
        {
            return NotFound();
        }

        state.Actions.QueueAction(character, action);

        return NoContent();
    }

    /// <summary>
    ///     Join combat
    /// </summary>
    [HttpPost("{monsterGroupGuid:guid}/join")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult JoinCombat(Guid characterGuid, Guid monsterGroupGuid)
    {
        Core.Game state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        MonsterGroupId monsterGroupId = new(monsterGroupGuid);
        JoinAndPlayPveCombatAction? action = state.Actions.GetAvailableActions(character)
            .OfType<JoinAndPlayPveCombatAction>()
            .SingleOrDefault(a => a.MonsterGroup.Id == monsterGroupId);

        if (action == null || action.Combat.Location != character.Location)
        {
            return NotFound();
        }

        state.Actions.QueueAction(character, action);

        return NoContent();
    }
}
