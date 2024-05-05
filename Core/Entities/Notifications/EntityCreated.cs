using MediatR;

namespace RestAdventure.Core.Entities.Notifications;

public class EntityCreated : INotification
{
    public required Entity Entity { get; init; }
}
