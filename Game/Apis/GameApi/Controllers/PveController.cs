using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;
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
        GameSnapshot state = _gameService.GetLastSnapshot();
        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        IReadOnlyCollection<Action> actions = state.Actions.GetAvailableActions(state, character);
        IEnumerable<StartAndPlayPveCombatAction> startCombatActions = actions.OfType<StartAndPlayPveCombatAction>();
        IEnumerable<JoinAndPlayPveCombatAction> joinCombatActions = actions.OfType<JoinAndPlayPveCombatAction>();

        List<MonsterGroupDto> result = [];

        foreach (JoinAndPlayPveCombatAction action in joinCombatActions)
        {
            if (state.Entities.GetValueOrDefault(action.MonsterGroupId) is not MonsterGroupSnapshot monsterGroup)
            {
                continue;
            }

            Maybe canStartCombat = character.Busy ? "Character is busy" : true;
            result.Add(
                new MonsterGroupDto
                {
                    Id = monsterGroup.Id.Guid,
                    Monsters = monsterGroup.Monsters.Select(m => m.ToDto()).ToArray(),
                    ExpectedExperience = monsterGroup.Monsters.Sum(m => m.Species.Experience),
                    Attacked = true,
                    CanAttackOrJoin = canStartCombat.Success,
                    WhyCannotAttackOrJoin = canStartCombat.WhyNot
                }
            );
        }

        foreach (StartAndPlayPveCombatAction action in startCombatActions)
        {
            if (state.Entities.GetValueOrDefault(action.MonsterGroupId) is not MonsterGroupSnapshot monsterGroup)
            {
                continue;
            }

            Maybe canStartCombat = character.Busy
                ? "Character is busy"
                : monsterGroup.OngoingCombatId != null
                    ? "Target is in combat"
                    : monsterGroup.Busy
                        ? "Target is busy"
                        : true;

            result.Add(
                new MonsterGroupDto
                {
                    Id = monsterGroup.Id.Guid,
                    Monsters = monsterGroup.Monsters.Select(m => m.ToDto()).ToArray(),
                    ExpectedExperience = monsterGroup.Monsters.Sum(m => m.Species.Experience),
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
        GameSnapshot state = _gameService.GetLastSnapshot();
        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        MonsterGroupId monsterGroupId = new(groupId);
        StartAndPlayPveCombatAction? action = state.Actions.GetAvailableActions(state, character)
            .OfType<StartAndPlayPveCombatAction>()
            .SingleOrDefault(a => a.MonsterGroupId == monsterGroupId);
        if (action == null)
        {
            return NotFound();
        }

        Core.Game game = _gameService.RequireGame();
        Character? gameCharacter = game.Entities.Get<Character>(characterId);
        if (gameCharacter == null)
        {
            return BadRequest();
        }

        game.Actions.QueueAction(gameCharacter, action);

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
        GameSnapshot state = _gameService.GetLastSnapshot();
        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        MonsterGroupId monsterGroupId = new(monsterGroupGuid);
        JoinAndPlayPveCombatAction? action = state.Actions.GetAvailableActions(state, character)
            .OfType<JoinAndPlayPveCombatAction>()
            .SingleOrDefault(a => a.MonsterGroupId == monsterGroupId);

        if (action == null)
        {
            return NotFound();
        }

        Core.Game game = _gameService.RequireGame();
        Character? gameCharacter = game.Entities.Get<Character>(characterId);
        if (gameCharacter == null)
        {
            return BadRequest();
        }

        game.Actions.QueueAction(gameCharacter, action);

        return NoContent();
    }
}
