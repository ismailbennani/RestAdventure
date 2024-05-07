using MediatR;

namespace RestAdventure.Core.Interactions.Notifications;

public class InteractionEnded: INotification
{
    public required InteractionInstance InteractionInstance { get; init; }
}
