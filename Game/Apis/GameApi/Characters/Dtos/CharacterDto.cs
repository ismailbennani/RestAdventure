using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Characters;

namespace RestAdventure.Game.Apis.GameApi.Characters.Dtos;

/// <summary>
///     Character
/// </summary>
public class CharacterDto
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
    public static CharacterDto ToDto(this CharacterDbo character) => new() { Id = character.Id, Name = character.Name, Class = character.Class };
}
