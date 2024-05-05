using MediatR;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Players.Notifications;

namespace RestAdventure.Core.Maps.Areas.Notifications;

public class DiscoverAreaOnLocationDiscovered : INotificationHandler<PlayerDiscoveredResource>
{
    public Task Handle(PlayerDiscoveredResource notification, CancellationToken cancellationToken)
    {
        if (notification.Resource is Location location)
        {
            notification.Player.Knowledge.Discover(location.Area);
        }

        return Task.CompletedTask;
    }
}
