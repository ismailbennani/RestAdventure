using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Interactions.Notifications;

namespace RestAdventure.Core.History.Characters;

public class CharacterEndedInteractionHistoryEntry : CharacterHistoryEntry
{
    public CharacterEndedInteractionHistoryEntry(InteractionInstance interactionInstance, long tick) : base(interactionInstance.Character, tick)
    {
        InteractionName = interactionInstance.Interaction.Name;
        TargetId = interactionInstance.Target.Id;
        TargetName = interactionInstance.Target.Name;
    }

    public string InteractionName { get; }
    public GameEntityId TargetId { get; }
    public string TargetName { get; }
}

public class CreateCharacterEndedInteractionHistoryEntry : INotificationHandler<InteractionEnded>
{
    readonly GameService _gameService;

    public CreateCharacterEndedInteractionHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(InteractionEnded notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        CharacterEndedInteractionHistoryEntry entry = new(notification.InteractionInstance, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
