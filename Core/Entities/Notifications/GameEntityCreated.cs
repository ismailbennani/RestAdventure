using MediatR;

namespace RestAdventure.Core.Entities.Notifications;

public class GameEntityCreated : INotification
{
    public required GameEntity Entity { get; init; }
}
