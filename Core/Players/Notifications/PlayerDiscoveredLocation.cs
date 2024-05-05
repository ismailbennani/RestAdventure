using MediatR;
using RestAdventure.Core.Maps;

namespace RestAdventure.Core.Players.Notifications;

public class PlayerDiscoveredLocation : INotification
{
    public required Player Player { get; init; }
    public required MapLocation Location { get; init; }

    public override string ToString() => $"{Player}[{Location}]";
}
