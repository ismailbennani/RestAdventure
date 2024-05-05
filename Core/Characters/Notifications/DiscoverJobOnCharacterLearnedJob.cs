using MediatR;

namespace RestAdventure.Core.Characters.Notifications;

public class DiscoverJobOnCharacterLearnedJob : INotificationHandler<CharacterLearnedJob>
{
    public Task Handle(CharacterLearnedJob notification, CancellationToken cancellationToken)
    {
        notification.Character.Team.Player.Knowledge.Discover(notification.Job);
        return Task.CompletedTask;
    }
}
