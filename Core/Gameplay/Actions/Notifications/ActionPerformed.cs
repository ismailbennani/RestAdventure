using MediatR;
using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Gameplay.Actions.Notifications;

public class ActionPerformed : INotification
{
    public required Character Character { get; init; }
    public required CharacterActionResult Result { get; init; }
}
