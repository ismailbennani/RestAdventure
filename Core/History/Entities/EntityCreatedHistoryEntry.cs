using MediatR;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Notifications;

namespace RestAdventure.Core.History.Entities;

public class EntityCreatedHistoryEntry : EntityHistoryEntry
{
    public EntityCreatedHistoryEntry(IGameEntity entity, long tick) : base(entity, tick)
    {
    }
}

public class CreateEntityCreatedHistoryEntry : INotificationHandler<GameEntityCreated>
{
    readonly GameService _gameService;

    public CreateEntityCreatedHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(GameEntityCreated notification, CancellationToken cancellationToken)
    {
        Game state = _gameService.RequireGameState();
        EntityCreatedHistoryEntry entry = new(notification.Entity, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
