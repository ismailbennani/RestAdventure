using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Characters.Notifications;

public class CharacterLearnedJob
{
    public required Character Character { get; init; }
    public required Job Job { get; init; }
}
