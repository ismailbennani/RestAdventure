using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters;

/// <summary>
///     Team of characters
/// </summary>
public class TeamDto
{
    /// <summary>
    ///     The characters in the team
    /// </summary>
    [Required]
    public required IReadOnlyCollection<CharacterDto> Characters { get; init; }
}
