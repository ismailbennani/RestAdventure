using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Players;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Core.Serialization.Jobs;
using RestAdventure.Core.Serialization.Players;
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
    [ProducesResponseType<IReadOnlyCollection<AvailableHarvestTargetDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<IReadOnlyCollection<AvailableHarvestTargetDto>> GetHarvestables(Guid characterGuid)
    {
        GameSnapshot state = _gameService.GetLastSnapshot();
        PlayerSnapshot player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        if (state.Entities.GetValueOrDefault(characterId) is not CharacterSnapshot character || character.PlayerId != player.UserId)
        {
            return BadRequest();
        }

        List<AvailableHarvestTargetDto> result = [];
        IEnumerable<StaticObjectInstanceSnapshot> staticObjects = state.Entities.Values.OfType<StaticObjectInstanceSnapshot>().Where(o => o.Location == character.Location);
        foreach (StaticObjectInstanceSnapshot staticObject in staticObjects)
        {
            List<AvailableHarvestActionDto> resultActions = [];

            foreach (JobInstanceSnapshot job in character.Jobs)
            foreach (JobHarvest harvest in job.Job.Harvests)
            {
                if (!harvest.CanTarget(staticObject))
                {
                    continue;
                }

                Maybe canHarvest = character.CanHarvestWithCorrectTool(harvest, staticObject);
                resultActions.Add(
                    new AvailableHarvestActionDto
                    {
                        Job = job.Job.ToMinimalDto(),
                        Name = harvest.Name,
                        Tool = harvest.Tool?.ToDto(),
                        ExpectedHarvest = harvest.Items.Select(i => i.ToDto()).ToArray(),
                        ExpectedExperience = harvest.Experience,
                        CanHarvest = canHarvest.Success,
                        WhyCannotHarvest = canHarvest.WhyNot
                    }
                );
            }

            result.Add(
                new AvailableHarvestTargetDto
                {
                    ObjectId = staticObject.Object.Id.Guid,
                    ObjectInstanceId = staticObject.Id.Guid,
                    Actions = resultActions
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
    public ActionResult Harvest(Guid characterGuid, Guid entityGuid, string harvestName, Guid? toolInstanceGuid = null)
    {
        Core.Game game = _gameService.RequireGame();
        Player player = ControllerContext.RequirePlayer(game);

        StaticObjectInstanceId staticObjectId = new(entityGuid);
        StaticObjectInstance? staticObject = game.Entities.Get<StaticObjectInstance>(staticObjectId);
        if (staticObject == null)
        {
            return NotFound();
        }

        CharacterId characterId = new(characterGuid);
        Character? character = game.Entities.Get<Character>(characterId);
        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        if (character.Location != staticObject.Location)
        {
            return NotFound();
        }

        var harvest = character.Jobs.SelectMany(j => j.Job.Harvests.Select(h => new { j.Job, Harvest = h })).FirstOrDefault(x => x.Harvest.Name == harvestName);
        if (harvest == null)
        {
            return NotFound();
        }

        ItemInstance? tool = null;
        if (toolInstanceGuid.HasValue)
        {
            ItemInstanceId toolInstanceId = new(toolInstanceGuid.Value);
            tool = character.Inventory.Stacks.FirstOrDefault(s => s.ItemInstance.Id == toolInstanceId)?.ItemInstance;
            if (tool == null)
            {
                return NotFound();
            }
        }

        HarvestAction action = new(harvest.Job, harvest.Harvest, staticObjectId, tool?.Id);
        Maybe queued = game.Actions.QueueAction(character, action);

        if (!queued.Success)
        {
            return Problem(queued.WhyNot, statusCode: StatusCodes.Status400BadRequest);
        }

        return NoContent();
    }
}
