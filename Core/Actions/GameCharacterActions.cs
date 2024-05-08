using MediatR;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Actions.Notifications;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Actions;

/// <summary>
///     Set the action that a character will perform on next tick.
/// </summary>
public class GameCharacterActions
{
    readonly GameState _state;
    readonly IPublisher _publisher;
    readonly ILogger<GameCharacterActions> _logger;
    readonly Dictionary<CharacterId, CharacterAction> _actions = new();
    readonly Dictionary<CharacterId, CharacterActionResult> _results = new();

    public GameCharacterActions(GameState state, ILogger<GameCharacterActions> logger)
    {
        _state = state;
        _publisher = state.Publisher;
        _logger = logger;
    }

    /// <summary>
    ///     Make the character move to the given location.
    /// </summary>
    public Maybe CanMoveTo(Character character, Location location)
    {
        Maybe canPerformAction = CanCharacterPerformAction(character);
        if (!canPerformAction)
        {
            return canPerformAction;
        }

        Maybe canMoveTo = character.Movement.CanMoveTo(_state, location);
        if (!canMoveTo)
        {
            return canMoveTo;
        }

        return true;
    }

    /// <summary>
    ///     Make the character move to the given location.
    /// </summary>
    public void PlanMovement(Character character, Location location)
    {
        if (!CanMoveTo(character, location))
        {
            throw new InvalidOperationException("Location is not accessible");
        }

        _actions[character.Id] = new CharacterMoveToLocationAction(location);
    }

    public Maybe CanInteract(Character character, Interaction interaction, IInteractibleEntity entity)
    {
        Maybe canPerformAction = CanCharacterPerformAction(character);
        if (!canPerformAction)
        {
            return canPerformAction;
        }

        Maybe canInteract = interaction.CanInteract(character, entity);
        if (!canInteract)
        {
            return canInteract;
        }

        return true;
    }

    /// <summary>
    ///     Perform an interaction.
    /// </summary>
    public void PlanInteraction(Character character, Interaction interaction, IInteractibleEntity entity)
    {
        Maybe canInteract = CanInteract(character, interaction, entity);
        if (!canInteract.Success)
        {
            throw new InvalidOperationException($"Interaction is not available: {canInteract.WhyNot}");
        }

        _actions[character.Id] = new CharacterInteractWithEntityAction(interaction, entity);
    }

    public void CancelCurrentPlan(Character character) => _actions.Remove(character.Id);

    public CharacterActionResult? GetLastActionResult(Character character) => _results.GetValueOrDefault(character.Id);

    public CharacterAction? GetPlannedAction(Character character) => _actions.GetValueOrDefault(character.Id);

    public async Task ResolveActionsAsync(GameState state)
    {
        _results.Clear();

        foreach ((CharacterId characterId, CharacterAction action) in _actions)
        {
            Character? character = state.Entities.Get<Character>(characterId);
            Maybe resolution;

            if (character == null)
            {
                resolution = "Character has been killed";
            }
            else
            {
                Maybe canPerform = action.CanPerform(state, character);
                if (!canPerform)
                {
                    resolution = canPerform;
                }
                else
                {
                    resolution = await action.PerformAsync(state, character);
                }
            }

            CharacterActionResult result = new()
            {
                Tick = state.Tick,
                Action = action,
                Success = resolution.Success,
                WhyNot = resolution.WhyNot
            };

            _results[characterId] = result;

            if (character != null)
            {
                await _publisher.Publish(new ActionPerformed { Character = character, Result = result });
            }
        }

        _actions.Clear();
    }

    static Maybe CanCharacterPerformAction(Character character)
    {
        if (character.CurrentInteraction != null)
        {
            return "Character is busy";
        }

        return true;
    }
}
