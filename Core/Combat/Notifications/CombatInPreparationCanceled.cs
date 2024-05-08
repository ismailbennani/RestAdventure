using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatInPreparationCanceled : INotification
{
    public required CombatInPreparation Combat { get; init; }
}
