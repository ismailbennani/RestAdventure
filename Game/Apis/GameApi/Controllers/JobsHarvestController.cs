using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
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
    readonly AvailableInteractionsService _availableInteractionsService;

    /// <summary>
    /// </summary>
    public JobsHarvestController(GameService gameService, AvailableInteractionsService availableInteractionsService)
    {
        _gameService = gameService;
        _availableInteractionsService = availableInteractionsService;
    }


    /// <summary>
    ///     Get harvestables
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<HarvestableEntityDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<HarvestableEntityDto>>> GetHarvestablesAsync(Guid characterGuid)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IEnumerable<IInteractibleEntity> entities = state.Entities.AtLocation<IInteractibleEntity>(character.Location);

        List<HarvestableEntityDto> result = [];
        foreach (IInteractibleEntity entity in entities)
        {
            HarvestInteraction[] availableInteractions = _availableInteractionsService.GetAvailableInteractions(character, entity).OfType<HarvestInteraction>().ToArray();
            if (availableInteractions.Length == 0)
            {
                continue;
            }

            List<HarvestableEntityHarvestDto> harvests = [];
            foreach (HarvestInteraction interaction in availableInteractions)
            {
                Maybe canHarvest = await interaction.CanInteractAsync(character, entity);
                harvests.Add(
                    new HarvestableEntityHarvestDto
                    {
                        Job = interaction.Job.ToMinimalDto(),
                        Name = interaction.Harvest.Name,
                        ExpectedHarvest = interaction.Harvest.Items.Select(i => i.ToDto()).ToArray(),
                        ExpectedExperience = interaction.Harvest.Experience,
                        CanHarvest = canHarvest.Success,
                        WhyCannotHarvest = canHarvest.WhyNot
                    }
                );
            }

            result.Add(
                new HarvestableEntityDto
                {
                    Id = entity.Id.Guid,
                    Name = entity.Name,
                    Harvests = harvests
                }
            );
        }

        return result;
    }

    /// <summary>
    ///     Harvest
    /// </summary>
    [HttpPost("{entityGuid:guid}/{harvest}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult Harvest(Guid characterGuid, Guid entityGuid, string harvest)
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
        IInteractibleEntity? entity = state.Entities.Get<IInteractibleEntity>(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        HarvestInteraction? interaction = _availableInteractionsService.GetAvailableInteractions(character, entity)
            .OfType<HarvestInteraction>()
            .SingleOrDefault(i => i.Harvest.Name == harvest);
        if (interaction == null)
        {
            return NotFound();
        }

        state.Actions.Interact(character, interaction, entity);

        return NoContent();
    }
}
