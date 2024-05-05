using MediatR;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Characters.Notifications;

public class CharacterLearnedJob : INotification
{
    public required Character Character { get; init; }
    public required Job Job { get; init; }

    public override string ToString() => $"{Character}[{Job}]";
}
