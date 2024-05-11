using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatEntityDied : INotification
{
    public required int SubTurn { get; init; }
    public required CombatInstance Combat { get; init; }
    public required ICombatEntity Attacker { get; init; }
    public required ICombatEntity Entity { get; init; }

    public override string ToString() => $"{Entity} killed by {Attacker}";
}
