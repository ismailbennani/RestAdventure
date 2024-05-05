using MediatR;

namespace RestAdventure.Game.MediatR;

class NotificationLogger<T> : INotificationHandler<T> where T: INotification
{
    readonly ILogger<NotificationLogger<T>> _logger;

    public NotificationLogger(ILogger<NotificationLogger<T>> logger)
    {
        _logger = logger;
    }

    public Task Handle(T notification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("{name} | {content}", typeof(T).Name, notification);
        return Task.CompletedTask;
    }
}
