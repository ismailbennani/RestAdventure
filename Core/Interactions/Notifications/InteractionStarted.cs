using MediatR;

namespace RestAdventure.Core.Interactions.Notifications;

public class InteractionStarted : INotification
{
    public required InteractionInstance InteractionInstance { get; init; }

    public override string ToString() => $"{InteractionInstance.Source}, {InteractionInstance.Interaction}, {InteractionInstance.Target}";
}
