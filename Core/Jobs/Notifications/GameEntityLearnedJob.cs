using MediatR;

namespace RestAdventure.Core.Jobs.Notifications;

public class GameEntityLearnedJob : INotification
{
    public required IGameEntityWithJobs Entity { get; init; }
    public required Job Job { get; init; }

    public override string ToString() => $"{Entity}[{Job}]";
}
