using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Players;
using RestAdventure.Core.StaticObjects;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.Jobs;
using RestAdventure.Game.Authentication;

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

        IEnumerable<StaticObjectInstance> entities = state.Entities.AtLocation<StaticObjectInstance>(character.Location);

        List<HarvestableEntityDto> result = [];
        foreach (StaticObjectInstance staticObject in entities)
        {
            List<HarvestableEntityHarvestDto> harvests = [];

            foreach (JobInstance job in character.Jobs)
            foreach (JobHarvest harvest in job.Harvests)
            {
                if (!harvest.Match(staticObject))
                {
                    continue;
                }

                bool canHarvest = job.Progression.Level >= harvest.Level;
                harvests.Add(
                    new HarvestableEntityHarvestDto
                    {
                        Job = job.Job.ToMinimalDto(),
                        Name = harvest.Name,
                        ExpectedHarvest = harvest.Items.Select(i => i.ToDto()).ToArray(),
                        ExpectedExperience = harvest.Experience,
                        CanHarvest = canHarvest,
                        WhyCannotHarvest = canHarvest ? null : "Job level too low"
                    }
                );
            }

            result.Add(
                new HarvestableEntityDto
                {
                    Id = staticObject.Id.Guid,
                    Name = staticObject.Name,
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

        var jobAndHarvest = character.Jobs.SelectMany(j => j.Harvests.Select(h => new { Job = j, Harvest = h })).SingleOrDefault(x => x.Harvest.Name == harvestName);
        if (jobAndHarvest == null)
        {
            return NotFound();
        }

        HarvestAction action = new(jobAndHarvest.Job.Job, jobAndHarvest.Harvest, entity);
        state.Actions.QueueAction(character, action);

        return NoContent();
    }
}
