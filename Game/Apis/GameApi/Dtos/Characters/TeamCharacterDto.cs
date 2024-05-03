using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Characters;
using RestAdventure.Game.Apis.GameApi.Dtos.Maps;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Characters;

/// <summary>
///     Character
/// </summary>
public class TeamCharacterDto
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

    /// <summary>
    ///     The current location of the character
    /// </summary>
    [Required]
    public required MapLocationDto Location { get; init; }
}

static class CharacterMappingExtensions
{
    public static TeamCharacterDto ToDto(this CharacterDbo character) =>
        new() { Id = character.Id, Name = character.Name, Class = character.Class, Location = character.Location.ToDto() };
}
