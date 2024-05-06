using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.GameApi.Controllers.Characters.Requests;

/// <summary>
///     Character creation options
/// </summary>
public class CreateCharacterRequestDto
{
    /// <summary>
    ///     The name of the character
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     The class of the character
    /// </summary>
    [Required]
    public required Guid ClassId { get; init; }
}
