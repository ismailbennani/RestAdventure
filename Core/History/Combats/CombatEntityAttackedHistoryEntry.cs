using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Entities;

namespace RestAdventure.Core.History.Combats;

public class CombatEntityAttackedHistoryEntry : CombatHistoryEntry
{
    public CombatEntityAttackedHistoryEntry(
        CombatInstance combat,
        IGameEntityWithCombatStatistics attacker,
        EntityAttack attack,
        IGameEntityWithCombatStatistics target,
        long tick
    ) : base(combat, tick)
    {
        AttackerId = attacker.Id;
        AttackerName = attacker.Name;
        TargetId = target.Id;
        TargetName = target.Name;
        Damage = attack.Damage;
    }

    public GameEntityId AttackerId { get; }
    public string AttackerName { get; }
    public GameEntityId TargetId { get; }
    public string TargetName { get; }
    public int Damage { get; }
}

public class CreateCombatEntityAttackedHistoryEntry : INotificationHandler<CombatEntityAttacked>
{
    readonly GameService _gameService;

    public CreateCombatEntityAttackedHistoryEntry(GameService gameService)
    {
        _gameService = gameService;
    }

    public Task Handle(CombatEntityAttacked notification, CancellationToken cancellationToken)
    {
        GameState state = _gameService.RequireGameState();
        CombatEntityAttackedHistoryEntry entry = new(notification.Combat, notification.Attacker, notification.AttackReceived, notification.Target, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
