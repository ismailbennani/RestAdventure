using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatEntityAttacked : INotification
{
    public required CombatInstance Combat { get; init; }
    public required int SubTurn { get; init; }
    public required ICombatEntity Attacker { get; init; }
    public required ICombatEntity Target { get; init; }
    public required CombatEntityAttack AttackDealt { get; init; }
    public required CombatEntityAttack AttackReceived { get; init; }

    public override string ToString() => $"-{AttackReceived.Damage} HP | {Attacker} -> {Target}";
}
