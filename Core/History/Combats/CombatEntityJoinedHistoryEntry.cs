using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Combat.Old;
using RestAdventure.Core.Entities;

namespace RestAdventure.Core.History.Combats;

public class CombatEntityJoinedHistoryEntry : CombatHistoryEntry
{
    public CombatEntityJoinedHistoryEntry(CombatInstance combat, IGameEntityWithCombatCapabilities entity, CombatSide side, long tick) : base(combat, tick)
    {
        EntityId = entity.Id;
        EntityName = entity.Name;
        Side = side;
    }

    public GameEntityId EntityId { get; }
    public string EntityName { get; }
    public CombatSide Side { get; }
}

public class CreateCombatEntityJoinedHistoryEntry : INotificationHandler<EntityJoinedCombat>
{
    readonly GameService _gameService;

    public CreateCombatEntityJoinedHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(EntityJoinedCombat notification, CancellationToken cancellationToken)
    {
        Game state = _gameService.RequireGameState();
        CombatEntityJoinedHistoryEntry entry = new(notification.Combat, notification.Entity, notification.Side, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
