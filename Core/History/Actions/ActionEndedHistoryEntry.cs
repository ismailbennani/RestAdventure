using MediatR;
using RestAdventure.Core.Actions.Notifications;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.History.Characters;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.History.Actions;

public class ActionEndedHistoryEntry : CharacterHistoryEntry
{
    public ActionEndedHistoryEntry(Character character, Action action, long tick) : base(character, tick)
    {
        ActionName = action.Name;
    }

    public string ActionName { get; }
}

public class CreateCharacterEndedInteractionHistoryEntry : INotificationHandler<ActionEnded>
{
    readonly GameService _gameService;

    public CreateCharacterEndedInteractionHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(ActionEnded notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        ActionEndedHistoryEntry entry = new(notification.Character, notification.Action, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
