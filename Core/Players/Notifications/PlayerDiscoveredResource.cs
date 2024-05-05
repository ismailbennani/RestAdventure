using MediatR;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Players.Notifications;

public class PlayerDiscoveredResource : INotification
{
    public required Player Player { get; init; }
    public required GameResource Resource { get; init; }

    public override string ToString() => $"{Player}[{Resource}]";
}
