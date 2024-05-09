using MediatR;

namespace RestAdventure.Core.Extensions;

static class PublisherExtensions
{
#pragma warning disable VSTHRD002
    public static void PublishSync(this IPublisher publisher, INotification notification) => publisher.Publish(notification).Wait();
#pragma warning restore VSTHRD002
}
