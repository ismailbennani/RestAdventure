using MediatR;

namespace RestAdventure.Core.Interactions.Notifications;

public class InteractionStarted: INotification
{
    public required InteractionInstance InteractionInstance { get; init; }
}
