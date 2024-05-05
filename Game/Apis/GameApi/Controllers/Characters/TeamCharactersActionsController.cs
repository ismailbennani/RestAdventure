using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Players;
using RestAdventure.Game.Authentication;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters;

/// <summary>
///     Characters actions operations
/// </summary>
[Route("game/team/characters/{characterGuid:guid}")]
[OpenApiTag("Team")]
public class TeamCharactersActionsController : GameApiController
{
    readonly GameService _gameService;
    readonly CharacterActionsService _characterActionsService;
    readonly CharacterInteractionsService _characterInteractionsService;

    /// <summary>
    /// </summary>
    public TeamCharactersActionsController(GameService gameService, CharacterActionsService characterActionsService, CharacterInteractionsService characterInteractionsService)
    {
        _gameService = gameService;
        _characterActionsService = characterActionsService;
        _characterInteractionsService = characterInteractionsService;
    }

    /// <summary>
    ///     Move to location
    /// </summary>
    [HttpPost("move/{locationGuid:guid}")]
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

        _characterActionsService.MoveToLocation(character, location);

        return NoContent();
    }

    /// <summary>
    ///     Interact
    /// </summary>
    [HttpPost("entity/{entityGuid:guid}/interact/{interactionGuid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> InteractAsync(Guid characterGuid, Guid entityGuid, Guid interactionGuid)
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

        await _characterInteractionsService.StartInteractionAsync(character, interaction, entity);

        return NoContent();
    }
}
