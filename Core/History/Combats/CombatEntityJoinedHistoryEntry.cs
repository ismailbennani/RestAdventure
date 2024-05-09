using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Entities;

namespace RestAdventure.Core.History.Combats;

public class CombatEntityJoinedHistoryEntry : CombatHistoryEntry
{
    public CombatEntityJoinedHistoryEntry(CombatInPreparation combat, IGameEntityWithCombatStatistics entity, CombatSide side, long tick) : base(combat, tick)
    {
        EntityId = entity.Id;
        EntityName = entity.Name;
        Side = side;
    }

    public GameEntityId EntityId { get; }
    public string EntityName { get; }
    public CombatSide Side { get; }
}

public class CreateCombatEntityJoinedHistoryEntry : INotificationHandler<CombatInPreparationEntityAdded>
{
    readonly GameService _gameService;

    public CreateCombatEntityJoinedHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatInPreparationEntityAdded notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        CombatEntityJoinedHistoryEntry entry = new(notification.CombatInPreparation, notification.Entity, notification.Side, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
