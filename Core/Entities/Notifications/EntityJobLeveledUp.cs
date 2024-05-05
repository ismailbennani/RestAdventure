using MediatR;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Entities.Notifications;

public class EntityJobLeveledUp : INotification
{
    public required IEntityWithJobs Entity { get; init; }
    public required Job Job { get; init; }
    public required int OldLevel { get; init; }
    public required int NewLevel { get; init; }
}
