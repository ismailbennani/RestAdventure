using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Jobs.Notifications;

namespace RestAdventure.Core.History.Entities;

public class EntityJobLeveledUpHistoryEntry : EntityHistoryEntry
{
    public EntityJobLeveledUpHistoryEntry(IGameEntity entity, Job job, int oldLevel, int newLevel, long tick) : base(entity, tick)
    {
        JobId = job.Id;
        JobName = job.Name;
        OldLevel = oldLevel;
        NewLevel = newLevel;
    }

    public JobId JobId { get; }
    public string JobName { get; }
    public int OldLevel { get; }
    public int NewLevel { get; }
}

public class CreateEntityJobLeveledUpHistoryEntry : INotificationHandler<GameEntityJobLeveledUp>
{
    readonly GameService _gameService;

    public CreateEntityJobLeveledUpHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityJobLeveledUp notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        EntityJobLeveledUpHistoryEntry entry = new(notification.Entity, notification.Job, notification.OldLevel, notification.NewLevel, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
