using MediatR;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.Spawners;

public class RemoveEntityFromSpawner : INotificationHandler<GameEntityDeleted>
{
    public Task Handle(GameEntityDeleted notification, CancellationToken cancellationToken)
    {
        if (notification.Entity.Source is Spawner spawner)
        {
            spawner.OnEntityDeleted(notification.Entity);
        }
        return Task.CompletedTask;
    }
}
