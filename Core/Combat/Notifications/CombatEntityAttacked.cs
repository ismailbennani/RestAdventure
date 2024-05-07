using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatEntityAttacked : INotification
{
    public required CombatInstance Combat { get; init; }
    public required int SubTurn { get; init; }
    public required IGameEntityWithCombatStatistics Attacker { get; init; }
    public required IGameEntityWithCombatStatistics Target { get; init; }
    public required int DamageDealt { get; init; }
    public required int DamageReceived { get; init; }
}
