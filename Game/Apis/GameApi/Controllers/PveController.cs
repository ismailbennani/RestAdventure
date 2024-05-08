using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Interactions;
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
    readonly AvailableInteractionsService _availableInteractionsService;

    /// <summary>
    /// </summary>
    public PveController(GameService gameService, AvailableInteractionsService availableInteractionsService)
    {
        _gameService = gameService;
        _availableInteractionsService = availableInteractionsService;
    }

    /// <summary>
    ///     Get monsters
    /// </summary>
    [HttpGet("monsters")]
    [ProducesResponseType<IReadOnlyCollection<MonsterGroupDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<MonsterGroupDto>>> GetMonstersAsync(Guid characterGuid)
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
            MonsterCombatInteraction? combatInteraction =
                _availableInteractionsService.GetAvailableInteractions(character, monster).OfType<MonsterCombatInteraction>().FirstOrDefault();
            if (combatInteraction == null)
            {
                continue;
            }

            Maybe canAttack = await combatInteraction.CanInteractAsync(character, monster);
            result.Add(new MonsterGroupDto { Id = monster.Id.Guid, Monsters = [monster.ToDto()], CanAttack = canAttack.Success, WhyCannotAttack = canAttack.WhyNot });
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

        MonsterCombatInteraction? interaction = _availableInteractionsService.GetAvailableInteractions(character, monster).OfType<MonsterCombatInteraction>().FirstOrDefault();
        if (interaction == null)
        {
            return NotFound();
        }

        state.Actions.Interact(character, interaction, monster);

        return NoContent();
    }
}
