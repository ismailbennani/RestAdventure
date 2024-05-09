using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.Jobs;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Game.Apis.GameApi.Controllers;

/// <summary>
///     Jobs operations
/// </summary>
[Route("game/team/characters/{characterGuid:guid}/jobs/harvestables")]
[OpenApiTag("Jobs")]
public class JobsHarvestController : GameApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public JobsHarvestController(GameService gameService)
    {
        _gameService = gameService;
    }


    /// <summary>
    ///     Get harvestables
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<HarvestableEntityDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyCollection<HarvestableEntityDto>> GetHarvestables(Guid characterGuid)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IEnumerable<HarvestAction> actions = state.Actions.GetAvailableActions(character).OfType<HarvestAction>();

        List<HarvestableEntityDto> result = [];
        foreach (IGrouping<StaticObjectInstance, HarvestAction> group in actions.GroupBy(a => a.Target))
        {
            List<HarvestableEntityHarvestDto> harvests = [];

            foreach (HarvestAction action in group)
            {
                Maybe canHarvest = action.CanPerform(state, character);
                harvests.Add(
                    new HarvestableEntityHarvestDto
                    {
                        Job = action.Job.ToMinimalDto(),
                        Name = action.Harvest.Name,
                        ExpectedHarvest = action.Harvest.Items.Select(i => i.ToDto()).ToArray(),
                        ExpectedExperience = action.Harvest.Experience,
                        CanHarvest = canHarvest.Success,
                        WhyCannotHarvest = canHarvest.WhyNot
                    }
                );
            }

            result.Add(
                new HarvestableEntityDto
                {
                    Id = group.Key.Id.Guid,
                    Name = group.Key.Name,
                    Harvests = harvests
                }
            );
        }

        return result;
    }

    /// <summary>
    ///     Harvest
    /// </summary>
    [HttpPost("{entityGuid:guid}/{harvestName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult Harvest(Guid characterGuid, Guid entityGuid, string harvestName)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        GameEntityId entityId = new(entityGuid);
        StaticObjectInstance? entity = state.Entities.Get<StaticObjectInstance>(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        HarvestAction? action = state.Actions.GetAvailableActions(character)
            .OfType<HarvestAction>()
            .SingleOrDefault(a => a.Harvest.Name == harvestName && a.Target.Id.Guid == entityGuid);
        if (action == null)
        {
            return NotFound();
        }

        state.Actions.QueueAction(character, action);

        return NoContent();
    }
}
