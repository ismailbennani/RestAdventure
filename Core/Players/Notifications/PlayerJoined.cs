using MediatR;

namespace RestAdventure.Core.Players.Notifications;

public class PlayerJoined : INotification
{
    public required Player Player { get; init; }

    public override string ToString() => $"{Player}";
}
