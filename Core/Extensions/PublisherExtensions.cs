using MediatR;

namespace RestAdventure.Core.Extensions;

static class PublisherExtensions
{
    public static void PublishSync(this IPublisher publisher, INotification notification) => publisher.Publish(notification).Wait();
}
