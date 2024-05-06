using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Characters;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters;

/// <summary>
///     Character class (minimal)
/// </summary>
public class CharacterClassMinimalDto
{
    /// <summary>
    ///     The unique ID of the character class
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the character class
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class CharacterClassMinimalMappingExtensions
{
    public static CharacterClassMinimalDto ToMinimalDto(this CharacterClass cls) =>
        new()
        {
            Id = cls.Id.Guid,
            Name = cls.Name
        };
}
