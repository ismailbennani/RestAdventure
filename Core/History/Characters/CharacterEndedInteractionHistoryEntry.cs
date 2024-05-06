using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Core.Gameplay.Interactions.Notifications;

namespace RestAdventure.Core.History.Characters;

public class CharacterEndedInteractionHistoryEntry : CharacterHistoryEntry
{
    public CharacterEndedInteractionHistoryEntry(InteractionInstance interactionInstance, long tick) : base(interactionInstance.Character, tick)
    {
        InteractionId = interactionInstance.Interaction.Id;
        InteractionName = interactionInstance.Interaction.Name;
        SubjectId = interactionInstance.Subject.Id;
        SubjectName = interactionInstance.Subject.Name;
    }

    public InteractionId InteractionId { get; }
    public string InteractionName { get; }
    public GameEntityId SubjectId { get; }
    public string SubjectName { get; }
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
