using MediatR;
using RestAdventure.Core.Items;

namespace RestAdventure.Core.Players.Notifications;

public class PlayerDiscoveredItem : INotification
{
    public required Player Player { get; init; }
    public required Item Item { get; init; }

    public override string ToString() => $"{Player}[{Item}]";
}
