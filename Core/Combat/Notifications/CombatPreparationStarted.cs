using MediatR;

namespace RestAdventure.Core.Combat.Notifications;

public class CombatPreparationStarted : INotification
{
    public required CombatInPreparation Combat { get; init; }

    public override string ToString() => $"{Combat}";
}
