using MediatR;
using RestAdventure.Core.Combat;
using RestAdventure.Core.Combat.Notifications;
using RestAdventure.Core.Combat.Old;
using CombatInstance = RestAdventure.Core.Combat.CombatInstance;

namespace RestAdventure.Core.History.Combats;

public class CombatEntityAttackedHistoryEntry : CombatHistoryEntry
{
    public CombatEntityAttackedHistoryEntry(CombatInstance combat, ICombatEntity attacker, CombatEntityAttack attack, ICombatEntity target, long tick) : base(combat, tick)
    {
        AttackerId = attacker.Id;
        AttackerName = attacker.Name;
        TargetId = target.Id;
        TargetName = target.Name;
        Damage = attack.Damage;
    }

    public CombatEntityId AttackerId { get; }
    public string AttackerName { get; }
    public CombatEntityId TargetId { get; }
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
        Game state = _gameService.RequireGame();
        CombatEntityAttackedHistoryEntry entry = new(notification.Combat, notification.Attacker, notification.AttackReceived, notification.Target, state.Tick);
        state.History.Record(entry);
        return Task.CompletedTask;
    }
}
