using MediatR;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.Characters.Notifications;

public class DiscoverItemsOnCharacterInventoryChanged : INotificationHandler<EntityInventoryChanged>
{
    public Task Handle(EntityInventoryChanged notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is not Character character)
        {
            return Task.CompletedTask;
        }

        character.Player.Knowledge.Discover(notification.ItemInstance.Item);
        return Task.CompletedTask;
    }
}
