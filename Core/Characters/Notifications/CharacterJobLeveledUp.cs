using MediatR;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Characters.Notifications;

public class CharacterJobLeveledUp : INotification
{
    public required Character Character { get; init; }
    public required Job Job { get; init; }
    public required int OldLevel { get; init; }
    public required int NewLevel { get; init; }
}
