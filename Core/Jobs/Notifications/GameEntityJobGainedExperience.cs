using MediatR;

namespace RestAdventure.Core.Jobs.Notifications;

public class GameEntityJobGainedExperience : INotification
{
    public required IGameEntityWithJobs Entity { get; init; }
    public required Job Job { get; init; }
    public required int OldExperience { get; init; }
    public required int NewExperience { get; init; }
}
