namespace RestAdventure.Core.Gameplay.Actions;

public class CharacterActionResult
{
    public required long Tick { get; init; }
    public required CharacterAction Action { get; init; }
    public required bool Success { get; init; }
    public string? FailureReason { get; init; }
}
