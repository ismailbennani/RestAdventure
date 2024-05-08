using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatPreparationStarted : INotification
{
    public required CombatInPreparation Combat { get; init; }
}
