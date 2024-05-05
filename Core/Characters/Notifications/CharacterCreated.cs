using MediatR;

namespace RestAdventure.Core.Characters.Notifications;

public class CharacterCreated : INotification
{
    public required Character Character { get; init; }

    public override string ToString() => $"{Character}";
}
