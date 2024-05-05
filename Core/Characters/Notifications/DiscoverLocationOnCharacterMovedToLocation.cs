using MediatR;

namespace RestAdventure.Core.Characters.Notifications;

public class DiscoverLocationOnCharacterMovedToLocation : INotificationHandler<CharacterMovedToLocation>
{
    public Task Handle(CharacterMovedToLocation notification, CancellationToken cancellationToken)
    {
        notification.Character.Team.Player.Knowledge.Discover(notification.Location);
        return Task.CompletedTask;
    }
}
