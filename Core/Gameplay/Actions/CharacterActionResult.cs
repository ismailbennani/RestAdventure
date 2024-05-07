using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Gameplay.Actions;

public class CharacterActionResult: Maybe
{
    public required long Tick { get; init; }
    public required CharacterAction Action { get; init; }
}
