using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.Jobs;
using RestAdventure.Game.Apis.Common.Dtos.StaticObjects;
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
        GameSnapshot state = _gameService.GetLastSnapshot();
        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        IEnumerable<HarvestAction> actions = state.Actions.GetAvailableActions(state, character).OfType<HarvestAction>();

        List<HarvestableEntityDto> result = [];
        foreach (IGrouping<StaticObjectInstanceId, HarvestAction> group in actions.GroupBy(a => a.TargetId))
        {
            List<HarvestableEntityHarvestDto> harvests = [];

            StaticObjectInstanceSnapshot? staticObject = state.Entities.GetValueOrDefault(group.Key) is StaticObjectInstanceSnapshot o ? o : null;
            Maybe canHarvest = staticObject != null ? staticObject.Busy ? "Target is busy" : true : true;

            foreach (HarvestAction action in group)
            {
                harvests.Add(
                    new HarvestableEntityHarvestDto
                    {
                        Job = action.Job.ToMinimalDto(),
                        Name = action.Harvest.Name,
                        Level = action.Harvest.Level,
                        Targets = action.Harvest.Targets.Select(t => t.ToStaticObjectDto()).ToArray(),
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
                    Id = group.Key.Guid,
                    Name = staticObject?.Name ?? "???",
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
        GameSnapshot state = _gameService.GetLastSnapshot();
        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        GameEntityId entityId = new(entityGuid);
        if (state.Entities.GetValueOrDefault(entityId) is not StaticObjectInstanceSnapshot)
        {
            return NotFound();
        }

        HarvestAction? action = state.Actions.GetAvailableActions(state, character)
            .OfType<HarvestAction>()
            .SingleOrDefault(a => a.Harvest.Name == harvestName && a.TargetId.Guid == entityGuid);
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
