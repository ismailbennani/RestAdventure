using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Actions.Providers;
using RestAdventure.Core.Characters;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Actions;

public class GameActions
{
    readonly GameState _state;
    readonly ILogger<GameActions> _logger;
    readonly ConcurrentDictionary<CharacterId, IReadOnlyCollection<Action>> _availableActions = new();
    Dictionary<CharacterId, Action> _queuedActions = new();
    readonly Dictionary<CharacterId, Action> _ongoingActions = new();
    readonly IReadOnlyCollection<IActionsProvider> _actionProviders;

    public GameActions(GameState state, IReadOnlyCollection<IActionsProvider> actionProviders, ILogger<GameActions> logger)
    {
        _state = state;
        _actionProviders = actionProviders;
        _logger = logger;
    }

    public IReadOnlyCollection<Action> GetAvailableActions(Character character) =>
        _availableActions.GetOrAdd(character.Id, _ => _actionProviders.SelectMany(p => p.GetActions(_state, character)).ToArray());

    public Maybe QueueAction(Character character, Action action)
    {
        Maybe canPerform = action.CanPerform(_state, character);
        if (!canPerform.Success)
        {
            return $"Character {character} cannot perform action {action}: {canPerform.WhyNot}";
        }

        _queuedActions[character.Id] = action;

        _logger.LogInformation("Action queued for {character}: {action} ({actionType})", character, action, action.GetType().Name);
        return true;
    }

    public async Task StartQueuedActionsAsync(GameState state)
    {
        const int fuel = 1000;
        for (int i = 0; i < fuel; i++)
        {
            Dictionary<CharacterId, Action> queuedActions = _queuedActions;
            _queuedActions = new Dictionary<CharacterId, Action>();

            foreach ((CharacterId characterId, Action queuedAction) in queuedActions)
            {
                Character? character = state.Entities.Get<Character>(characterId);
                if (character == null)
                {
                    continue;
                }

                Maybe canPerform = queuedAction.CanPerform(state, character);
                if (!canPerform.Success)
                {
                    _logger.LogError("Could not perform action {action}: {reason}", queuedAction, canPerform.WhyNot);
                    continue;
                }

                await queuedAction.StartAsync(state, character);
                _ongoingActions[character.Id] = queuedAction;
            }

            if (_queuedActions.Count == 0)
            {
                return;
            }
        }

        _logger.LogWarning("Queued actions loop aborted, is there a cycle ?");
    }

    public async Task TickOngoingActionsAsync(GameState state)
    {
        foreach ((CharacterId characterId, Action action) in _ongoingActions)
        {
            Character? character = state.Entities.Get<Character>(characterId);
            if (character == null)
            {
                continue;
            }

            await action.TickAsync(state, character);
        }
    }

    public async Task RemoveFinishedActionsAsync(GameState state)
    {
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

    public void OnTickEnd(GameState state) => _availableActions.Clear();

    public Action? GetQueuedAction(Character character) => _queuedActions.GetValueOrDefault(character.Id);
    public Action? GetOngoingAction(Character character) => _ongoingActions.GetValueOrDefault(character.Id);
}
