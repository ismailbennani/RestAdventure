using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatEnded: INotification
{
    public required CombatInstance Combat { get; init; }
}
