using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.Spawners.Notifications;

public class RemoveEntityFromSpawner : INotificationHandler<GameEntityDeleted>
{
    public Task Handle(GameEntityDeleted notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is GameEntity { Source: Spawner spawner })
        {
            spawner.OnEntityDeleted(notification.Entity);
        }

        return Task.CompletedTask;
    }
}
