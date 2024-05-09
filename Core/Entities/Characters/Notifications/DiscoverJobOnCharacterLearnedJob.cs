using MediatR;
using RestAdventure.Core.Jobs.Notifications;

namespace RestAdventure.Core.Entities.Characters.Notifications;

public class DiscoverJobOnCharacterLearnedJob : INotificationHandler<GameEntityLearnedJob>
{
    public Task Handle(GameEntityLearnedJob notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is not Character character)
        {
            return Task.CompletedTask;
        }

        character.Player.Knowledge.Discover(notification.Job);
        return Task.CompletedTask;
    }
}
