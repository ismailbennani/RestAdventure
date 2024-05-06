using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Characters;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters;

/// <summary>
///     Character class
/// </summary>
public class CharacterClassDto : CharacterClassMinimalDto
{
    /// <summary>
    ///     The description of the character class
    /// </summary>
    [Required]
    public string? Description { get; init; }

    /// <summary>
    ///     The level caps of the character class
    /// </summary>
    [Required]
    public required IReadOnlyList<int> LevelCaps { get; init; }
}

static class CharacterClassMappingExtensions
{
    public static CharacterClassDto ToDto(this CharacterClass cls) =>
        new()
        {
            Id = cls.Id.Guid,
            Name = cls.Name,
            Description = cls.Description,
            LevelCaps = cls.LevelCaps
        };
}
