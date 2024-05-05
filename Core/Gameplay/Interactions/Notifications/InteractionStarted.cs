﻿using MediatR;

namespace RestAdventure.Core.Gameplay.Interactions.Notifications;

public class InteractionStarted: INotification
{
    public required InteractionInstance InteractionInstance { get; init; }
}
