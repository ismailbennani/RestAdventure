using MediatR;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.Entities.Characters.Notifications;

public class DiscoverItemsOnCharacterInventoryChanged : INotificationHandler<GameEntityInventoryChanged>
{
    public Task Handle(GameEntityInventoryChanged notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is not Character character)
        {
            return Task.CompletedTask;
        }

        character.Player.Knowledge.Discover(notification.ItemInstance.Item);
        return Task.CompletedTask;
    }
}
