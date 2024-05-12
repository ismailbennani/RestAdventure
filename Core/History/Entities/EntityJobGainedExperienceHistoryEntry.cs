using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Jobs.Notifications;

namespace RestAdventure.Core.History.Entities;

public class EntityJobGainedExperienceHistoryEntry : EntityHistoryEntry
{
    public EntityJobGainedExperienceHistoryEntry(IGameEntity entity, Job job, int oldExperience, int newExperience, long tick) : base(entity, tick)
    {
        JobId = job.Id;
        JobName = job.Name;
        OldExperience = oldExperience;
        NewExperience = newExperience;
    }

    public JobId JobId { get; }
    public string JobName { get; }
    public int OldExperience { get; }
    public int NewExperience { get; }
}

public class CreateEntityJobGainedExperienceHistoryEntry : INotificationHandler<GameEntityJobGainedExperience>
{
    readonly GameService _gameService;

    public CreateEntityJobGainedExperienceHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityJobGainedExperience notification, CancellationToken cancellationToken)
    {
        Game state = _gameService.RequireGame();
        EntityJobGainedExperienceHistoryEntry entry = new(notification.Entity, notification.Job, notification.OldExperience, notification.NewExperience, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
