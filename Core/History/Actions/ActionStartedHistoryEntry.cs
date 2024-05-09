using MediatR;
using RestAdventure.Core.Actions.Notifications;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.History.Characters;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.History.Actions;

public class ActionStartedHistoryEntry : CharacterHistoryEntry
{
    public ActionStartedHistoryEntry(Character character, Action action, long tick) : base(character, tick)
    {
        ActionName = action.Name;
    }

    public string ActionName { get; }
}

public class CreateCharacterStartedInteractionHistoryEntry : INotificationHandler<ActionStarted>
{
    readonly GameService _gameService;

    public CreateCharacterStartedInteractionHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(ActionStarted notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        ActionStartedHistoryEntry entry = new(notification.Character, notification.Action, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
