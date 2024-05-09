using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Entities;

namespace RestAdventure.Core.History.Combats;

public class CombatEntityDiedHistoryEntry : CombatHistoryEntry
{
    public CombatEntityDiedHistoryEntry(CombatInstance combat, IGameEntityWithCombatStatistics entity, IGameEntityWithCombatStatistics attacker, long tick) : base(combat, tick)
    {
        EntityId = entity.Id;
        EntityName = entity.Name;
        AttackerId = attacker.Id;
        AttackerName = attacker.Name;
    }

    public GameEntityId EntityId { get; }
    public string EntityName { get; }
    public GameEntityId AttackerId { get; }
    public string AttackerName { get; }
}

public class CreateCombatEntityDiedHistoryEntry : INotificationHandler<CombatEntityDied>
{
    readonly GameService _gameService;

    public CreateCombatEntityDiedHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatEntityDied notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        CombatEntityDiedHistoryEntry entry = new(notification.Combat, notification.Entity, notification.Attacker, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
