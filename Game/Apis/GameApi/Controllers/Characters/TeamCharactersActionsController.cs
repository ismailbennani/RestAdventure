using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Players;
using RestAdventure.Game.Apis.Common.Dtos.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Interactions;
using RestAdventure.Game.Apis.Common.Dtos.Maps;
using RestAdventure.Game.Authentication;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

/// <summary>
///     Characters actions operations
/// </summary>
[Route("game/team/characters/{characterGuid:guid}")]
[OpenApiTag("Team")]
public class TeamCharactersActionsController : GameApiController
{
    readonly GameService _gameService;

    /// <summary>
    /// </summary>
    public TeamCharactersActionsController(GameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    ///     Get accessible locations
    /// </summary>
    [HttpGet("locations")]
    public ActionResult<IReadOnlyCollection<LocationMinimalDto>> GetAccessibleLocations(Guid characterGuid)
    {
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IEnumerable<Location> accessibleLocations = content.Maps.Locations.ConnectedTo(character.Location);
        return accessibleLocations.Select(l => l.ToMinimalDto()).ToArray();
    }

    /// <summary>
    ///     Move to location
    /// </summary>
    [HttpPost("locations/{locationGuid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult MoveToLocation(Guid characterGuid, Guid locationGuid)
    {
        GameContent content = _gameService.RequireGameContent();
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        LocationId locationId = new(locationGuid);
        Location? location = content.Maps.Locations.Get(locationId);
        if (location == null)
        {
            return NotFound();
        }

        state.Actions.MoveToLocation(character, location);

        return NoContent();
    }

    /// <summary>
    ///     Get available interactions
    /// </summary>
    [HttpGet("interactions")]
    public async Task<ActionResult<IReadOnlyCollection<EntityWithInteractionsDto>>> GetAvailableInteractionsAsync(Guid characterGuid)
    {
        GameState state = _gameService.RequireGameState();
        Player player = ControllerContext.RequirePlayer(state);

        CharacterId characterId = new(characterGuid);
        Character? character = state.Entities.Get<Character>(characterId);

        if (character == null || character.Player != player)
        {
            return BadRequest();
        }

        IEnumerable<IGameEntityWithInteractions> entities = state.Entities.AtLocation<IGameEntityWithInteractions>(character.Location);

        List<EntityWithInteractionsDto> result = [];
        foreach (IGameEntityWithInteractions entity in entities)
        {
            List<InteractionDto> interactions = [];
            foreach (Interaction interaction in entity.Interactions.All)
            {
                Maybe canInteract = await interaction.CanInteractAsync(state, character, entity);
                interactions.Add(interaction.ToDto(canInteract));
            }

            result.Add(
                new EntityWithInteractionsDto
                {
                    Id = entity.Id.Guid,
                    Name = entity.Name,
                    Interactions = interactions
                }
            );
        }


        return result;
    }

    /// <summary>
    ///     Interact
    /// </summary>
    [HttpPost("interactions/entity/{entityGuid:guid}/{interactionGuid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult Interact(Guid characterGuid, Guid entityGuid, Guid interactionGuid)
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
        IGameEntityWithInteractions? entity = state.Entities.Get<IGameEntityWithInteractions>(entityId);
        if (entity == null)
        {
            return NotFound();
        }

        InteractionId interactionId = new(interactionGuid);
        Interaction? interaction = entity.Interactions.Get(interactionId);
        if (interaction == null)
        {
            return NotFound();
        }

        state.Actions.Interact(character, interaction, entity);

        return NoContent();
    }
}
