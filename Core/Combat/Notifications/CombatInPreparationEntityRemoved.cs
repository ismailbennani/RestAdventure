using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatInPreparationEntityRemoved : INotification
{
    public required CombatInPreparation CombatInPreparation { get; init; }
    public required CombatSide Side { get; init; }
    public required IGameEntityWithCombatStatistics Entity { get; init; }
}
