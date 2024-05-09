using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatEntityDied : INotification
{
    public required int SubTurn { get; init; }
    public required CombatInstance Combat { get; init; }
    public required IGameEntityWithCombatStatistics Attacker { get; init; }
    public required IGameEntityWithCombatStatistics Entity { get; init; }
}
