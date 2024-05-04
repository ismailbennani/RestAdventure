using System.Diagnostics.CodeAnalysis;

namespace RestAdventure.Core.Characters;

public class CharacterCreationResult
{
    /// <summary>
    ///     Has the creation been successful
    /// </summary>
    [MemberNotNullWhen(true, nameof(Character))]
    [MemberNotNullWhen(false, nameof(ErrorMessage))]
    public required bool IsSuccess { get; init; }

    /// <summary>
    ///     If the creation succeeded, the character that was created
    /// </summary>
    public Character? Character { get; init; }

    /// <summary>
    ///     If the creation failed, the reason why
    /// </summary>
    public string? ErrorMessage { get; init; }
}
