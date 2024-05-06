using MediatR;

namespace RestAdventure.Core.Jobs.Notifications;

public class GameEntityJobLeveledUp : INotification
{
    public required IGameEntityWithJobs Entity { get; init; }
    public required Job Job { get; init; }
    public required int OldLevel { get; init; }
    public required int NewLevel { get; init; }
}
