using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatPreparationStarted : INotification
{
    public required CombatInstance Combat { get; init; }

    public override string ToString() => $"{Combat}";
}
