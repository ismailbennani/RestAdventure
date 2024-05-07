using MediatR;

namespace RestAdventure.Core.Entities.Notifications;

public class GameEntityDeleted : INotification
{
    public required IGameEntity Entity { get; init; }

    public override string ToString() => $"{Entity}";
}
