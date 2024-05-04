namespace RestAdventure.Core.Gameplay.Actions;

public class CharacterActionResolution
{
    public required bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}