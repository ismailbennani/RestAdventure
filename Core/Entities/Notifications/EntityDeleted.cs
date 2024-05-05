using MediatR;

namespace RestAdventure.Core.Entities.Notifications;

public class EntityDeleted : INotification
{
    public required Entity Entity { get; init; }

    public override string ToString() => $"{Entity}";
}
