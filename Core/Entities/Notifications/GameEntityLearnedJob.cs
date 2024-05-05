using MediatR;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Entities.Notifications;

public class GameEntityLearnedJob : INotification
{
    public required IEntityWithJobs Entity { get; init; }
    public required Job Job { get; init; }

    public override string ToString() => $"{Entity}[{Job}]";
}
