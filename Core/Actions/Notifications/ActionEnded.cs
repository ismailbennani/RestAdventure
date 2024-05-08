using MediatR;
using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Actions.Notifications;

public class ActionEnded : INotification
{
    public required Character Character { get; init; }
    public required Action Action { get; init; }
}
