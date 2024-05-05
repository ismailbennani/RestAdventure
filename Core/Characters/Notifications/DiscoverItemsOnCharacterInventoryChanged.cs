using MediatR;

namespace RestAdventure.Core.Characters.Notifications;

public class DiscoverItemsOnCharacterInventoryChanged : INotificationHandler<CharacterInventoryChanged>
{
    public Task Handle(CharacterInventoryChanged notification, CancellationToken cancellationToken)
    {
        notification.Character.Team.Player.Discover(notification.ItemInstance.Item);
        return Task.CompletedTask;
    }
}
