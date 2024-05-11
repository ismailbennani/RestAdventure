using MediatR;
using RestAdventure.Core.Combat.Old;

namespace RestAdventure.Core.Combat.Notifications;

public class EntityLeftCombat : INotification
{
    public required CombatInstance Combat { get; init; }
    public required CombatSide Side { get; init; }
    public required IGameEntityWithCombatCapabilities Entity { get; init; }
}
