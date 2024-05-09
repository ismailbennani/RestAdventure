using MediatR;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.Entities.Characters.Notifications;

public class DiscoverLocationOnCharacterMovedToLocation : INotificationHandler<GameEntityMovedToLocation>
{
    public Task Handle(GameEntityMovedToLocation notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is not Character character)
        {
            return Task.CompletedTask;
        }

        character.Player.Knowledge.Discover(notification.NewLocation);
        return Task.CompletedTask;
    }
}
