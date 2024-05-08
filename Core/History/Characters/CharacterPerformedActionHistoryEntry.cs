using MediatR;
using RestAdventure.Core.Actions;
using RestAdventure.Core.Actions.Notifications;
using RestAdventure.Core.Characters;

namespace RestAdventure.Core.History.Characters;

public class CharacterPerformedActionHistoryEntry : CharacterHistoryEntry
{
    public CharacterPerformedActionHistoryEntry(Character source, CharacterActionResult result) : base(source, result.Tick)
    {
        Action = result.Action;
        Success = result.Success;
        FailureReason = result.WhyNot;
    }

    public CharacterAction Action { get; }
    public bool Success { get; }
    public string? FailureReason { get; }
}

public class CreateCharacterPerformedActionHistoryEntry : INotificationHandler<ActionPerformed>
{
    readonly GameService _gameService;

    public CreateCharacterPerformedActionHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(ActionPerformed notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        CharacterPerformedActionHistoryEntry entry = new(notification.Character, notification.Result);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
