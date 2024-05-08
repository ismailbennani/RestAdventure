using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Combat.Pve;
using RestAdventure.Core.Monsters;
using RestAdventure.Core.Players;
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

        IEnumerable<MonsterInstance> monsters = state.Entities.AtLocation<MonsterInstance>(character.Location);

        List<MonsterGroupDto> result = new();
        foreach (MonsterInstance monster in monsters)
        {
            Maybe canStartCombat = state.Combats.CanStartCombat([character], [monster]);
            result.Add(
                new MonsterGroupDto
                {
                    Id = monster.Id.Guid, Monsters = [monster.ToDto()], CanAttack = canStartCombat.Success, WhyCannotAttack = canStartCombat.WhyNot,
                    ExpectedExperience = monster.Species.Experience
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
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        MonsterInstanceId monsterId = new(groupId);
        MonsterInstance? monster = state.Entities.Get<MonsterInstance>(monsterId);
        if (monster == null)
        {
            return NotFound();
        }

        StartPveCombatAction action = new([character], [monster], _loggerFactory.CreateLogger<StartPveCombatAction>());
        state.Actions.QueueAction(character, action);

        return NoContent();
    }
}
