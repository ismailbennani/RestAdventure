﻿using Microsoft.Extensions.Logging;
using RestAdventure.Core.Characters;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Actions;

public class GameActions
{
    readonly GameState _state;
    readonly ILogger<GameActions> _logger;
    readonly Dictionary<CharacterId, Action> _queuedActions = new();
    readonly Dictionary<CharacterId, Action> _ongoingActions = new();

    public GameActions(GameState state, ILogger<GameActions> logger)
    {
        _state = state;
        _logger = logger;
    }

    public Maybe QueueAction(Character character, Action action)
    {
        Maybe canPerform = action.CanPerform(_state, character);
        if (!canPerform.Success)
        {
            return $"Character {character} cannot perform action {action}: {canPerform.WhyNot}";
        }

        _queuedActions[character.Id] = action;
        return true;
    }

    public async Task ResolveActionsAsync(GameState state)
    {
        foreach ((CharacterId characterId, Action queuedAction) in _queuedActions)
        {
            Character? character = state.Entities.Get<Character>(characterId);
            if (character == null)
            {
                continue;
            }

            await queuedAction.StartAsync(state, character);
            _ongoingActions[character.Id] = queuedAction;
        }
        _queuedActions.Clear();

        foreach ((CharacterId characterId, Action action) in _ongoingActions)
        {
            Character? character = state.Entities.Get<Character>(characterId);
            if (character == null)
            {
                continue;
            }

            await action.TickAsync(state, character);
        }

        List<CharacterId> toRemove = [];
        foreach ((CharacterId characterId, Action instance) in _ongoingActions)
        {
            Character? character = state.Entities.Get<Character>(characterId);
            if (character == null)
            {
                _logger.LogWarning("Character performing action {action} not found. Action will be removed without calling {onEnd}", instance, nameof(Action.EndAsync));
                toRemove.Add(characterId);
                continue;
            }

            if (instance.IsOver(state, character))
            {
                await instance.EndAsync(state, character);
                toRemove.Add(characterId);
            }
        }

        foreach (CharacterId characterId in toRemove)
        {
            if (!_ongoingActions.Remove(characterId))
            {
                _logger.LogWarning("Could not remove action of character {characterId}", characterId);
            }

        }
    }

    public Action? GetQueuedAction(Character character) => _queuedActions.GetValueOrDefault(character.Id);
    public Action? GetOngoingAction(Character character) => _ongoingActions.GetValueOrDefault(character.Id);
}
