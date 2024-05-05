using MediatR;

namespace RestAdventure.Core.Gameplay.Interactions.Notifications;

public class InteractionEnded: INotification
{
    public required InteractionInstance InteractionInstance { get; init; }
}
