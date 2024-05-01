namespace Server.Characters.Dtos;

/// <summary>
///     Character
/// </summary>
public class CharacterDto
{
    /// <summary>
    ///     The unique ID of the character
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     The name of the character
    /// </summary>
    public required string Name { get; init; }
}
