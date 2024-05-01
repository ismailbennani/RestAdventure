namespace Server.Characters.Dtos;

/// <summary>
///     Team of characters
/// </summary>
public class TeamDto
{
    /// <summary>
    ///     The characters in the team
    /// </summary>
    public required IReadOnlyCollection<CharacterDto> Characters { get; init; }
}
