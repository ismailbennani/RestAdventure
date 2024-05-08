using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatInPreparationEntitiesReordered : INotification
{
    public required CombatInPreparation CombatInPreparation { get; init; }
    public required CombatSide Side { get; init; }
    public required IReadOnlyList<IGameEntityWithCombatStatistics> OldOrder { get; init; }
    public required IReadOnlyList<IGameEntityWithCombatStatistics> NewOrder { get; init; }
}
