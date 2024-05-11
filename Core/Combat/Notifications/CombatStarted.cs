using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatStarted : INotification
{
    public required CombatInstance Combat { get; init; }

    public override string ToString() => $"{Combat}";
}
