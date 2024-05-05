﻿using MediatR;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.Characters.Notifications;

public class DiscoverJobOnCharacterLearnedJob : INotificationHandler<EntityLearnedJob>
{
    public Task Handle(EntityLearnedJob notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is not Character character)
        {
            return Task.CompletedTask;
        }

        character.Player.Knowledge.Discover(notification.Job);
        return Task.CompletedTask;
    }
}
