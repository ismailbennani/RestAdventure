using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Jobs.Notifications;

namespace RestAdventure.Core.History.Entities;

public class EntityLearnedJobHistoryEntry : EntityHistoryEntry
{
    public EntityLearnedJobHistoryEntry(IGameEntity entity, Job job, long tick) : base(entity, tick)
    {
        JobId = job.Id;
        JobName = job.Name;
    }

    public JobId JobId { get; }
    public string JobName { get; }
}

public class CreateEntityLearnedJobHistoryEntry : INotificationHandler<GameEntityLearnedJob>
{
    readonly GameService _gameService;

    public CreateEntityLearnedJobHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityLearnedJob notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        EntityLearnedJobHistoryEntry entry = new(notification.Entity, notification.Job, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
