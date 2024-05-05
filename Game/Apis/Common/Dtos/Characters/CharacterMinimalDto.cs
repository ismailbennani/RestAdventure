using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Characters;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters;

/// <summary>
///     Minimal character information
/// </summary>
public class CharacterMinimalDto
{
    /// <summary>
    ///     The unique ID of the character
    /// </summary>
    [Required]
    public Guid Id { get; init; }

    /// <summary>
    ///     The name of the character
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     The class of the character
    /// </summary>
    [Required]
    public required CharacterClass Class { get; init; }
}

static class CharacterMappingExtensions
{
    public static CharacterMinimalDto ToMinimalCharacterDto(this Character character) => new() { Id = character.Id.Guid, Name = character.Name, Class = character.Class };
}
