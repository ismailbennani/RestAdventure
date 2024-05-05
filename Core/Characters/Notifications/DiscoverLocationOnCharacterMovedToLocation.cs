using MediatR;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.Characters.Notifications;

public class DiscoverLocationOnCharacterMovedToLocation : INotificationHandler<EntityMovedToLocation>
{
    public Task Handle(EntityMovedToLocation notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is not Character character)
        {
            return Task.CompletedTask;
        }

        character.Player.Knowledge.Discover(notification.NewLocation);
        return Task.CompletedTask;
    }
}
