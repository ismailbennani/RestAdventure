using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Old;
using RestAdventure.Core.Combat.Options;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Players;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Combats in preparation operations
/// </summary>
[Route("game/team/characters/{characterGuid:guid}/combats/preparation")]
[OpenApiTag("Combats")]
public class CombatsInPreparationController : GameApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public CombatsInPreparationController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Set combat in preparation options
    /// </summary>
    [HttpPost("{combatGuid:guid}/{side}/options/accessibility")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult SetCombatInPreparationAccessibility(Guid characterGuid, Guid combatGuid, CombatSide side, CombatFormationAccessibility value)
    {
        Core.Game state = _gameService.RequireGame();
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

        CombatFormation formation = combat.GetTeam(side);
        Maybe performed = formation.SetOptions(character, new CombatFormationOptions { Accessibility = value, MaxCount = formation.Options.MaxCount });
        if (!performed.Success)
        {
            return Problem(performed.WhyNot, statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }
}
