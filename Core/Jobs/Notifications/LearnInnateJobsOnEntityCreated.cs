using MediatR;
using RestAdventure.Core.Content;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.Jobs.Notifications;

public class LearnInnateJobsOnEntityCreated : INotificationHandler<GameEntityCreated>
{
    readonly GameService _gameService;

    public LearnInnateJobsOnEntityCreated(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityCreated notification, CancellationToken cancellationToken)
    {
        if (notification.Entity is not IGameEntityWithJobs withJobs)
        {
            return Task.CompletedTask;
        }

        GameContent content = _gameService.RequireGameContent();

        foreach (Job job in content.Jobs.Innate)
        {
            withJobs.Jobs.Learn(job);
        }

        return Task.CompletedTask;
    }
}
