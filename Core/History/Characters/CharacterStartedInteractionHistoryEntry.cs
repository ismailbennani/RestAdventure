using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Interactions.Notifications;

namespace RestAdventure.Core.History.Characters;

public class CharacterStartedInteractionHistoryEntry : CharacterHistoryEntry
{
    public CharacterStartedInteractionHistoryEntry(InteractionInstance interactionInstance, long tick) : base(interactionInstance.Character, tick)
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

public class CreateCharacterStartedInteractionHistoryEntry : INotificationHandler<InteractionStarted>
{
    readonly GameService _gameService;

    public CreateCharacterStartedInteractionHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(InteractionStarted notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        CharacterStartedInteractionHistoryEntry entry = new(notification.InteractionInstance, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
