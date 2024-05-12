using Microsoft.Extensions.Logging;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Actions;

public class GameActions
{
    readonly Game _state;
    readonly ILogger<GameActions> _logger;
    readonly Dictionary<CharacterId, Action> _queuedActions = new();
    readonly Dictionary<CharacterId, Action> _ongoingActions = new();

    public GameActions(Game state, IReadOnlyCollection<IActionsProvider> actionProviders)
    {
        _state = state;
        ActionProviders = actionProviders;
        _logger = state.LoggerFactory.CreateLogger<GameActions>();
    }

    internal IReadOnlyCollection<IActionsProvider> ActionProviders { get; }

    public IEnumerable<KeyValuePair<CharacterId, Action>> Queued => _queuedActions;
    public IEnumerable<KeyValuePair<CharacterId, Action>> Ongoing => _ongoingActions;

    public Maybe QueueAction(Character character, Action action)
    {
        Maybe canPerform = action.CanPerform(_state, character);
        if (!canPerform.Success)
        {
            return canPerform.WhyNot;
        }

        _queuedActions[character.Id] = action;

        _logger.LogDebug("Action queued for {character}: {action} ({actionType})", character, action, action.GetType().Name);
        return true;
    }

    public Action? GetQueuedAction(Character character) => _queuedActions.GetValueOrDefault(character.Id);

    public void RemoveQueuedAction(Character character, Action action)
    {
        if (_queuedActions.GetValueOrDefault(character.Id) != action)
        {
            return;
        }

        _queuedActions.Remove(character.Id);
    }

    public async Task<Maybe> StartQueuedActionAsync(Character character)
    {
        if (!_queuedActions.Remove(character.Id, out Action? action))
        {
            return "Could not find action queued for character";
        }

        Maybe canPerform = action.CanPerform(_state, character);
        if (!canPerform.Success)
        {
            return canPerform.WhyNot;
        }

        await action.StartAsync(_state, character);
        _ongoingActions[character.Id] = action;

        return true;
    }

    public async Task<Maybe> TickOngoingActionAsync(Character character)
    {
        if (!_ongoingActions.TryGetValue(character.Id, out Action? action))
        {
            return "Could not find ongoing action for character";
        }

        await action.TickAsync(_state, character);
        return true;
    }

    public Action? GetOngoingAction(Character character) => _ongoingActions.GetValueOrDefault(character.Id);

    public async Task<Maybe> EndOngoingActionAsync(Character character)
    {
        if (!_ongoingActions.TryGetValue(character.Id, out Action? action))
        {
            return "Could not find ongoing action for character";
        }

        await action.EndAsync(_state, character);
        _ongoingActions.Remove(character.Id);

        return true;
    }

    public void ClearTickCache() => _queuedActions.Clear();
}
