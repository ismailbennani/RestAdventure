﻿using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Characters;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Characters;

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

static class OtherCharacterMappingExtensions
{
    public static CharacterDto ToOtherCharacterDto(this CharacterDbo character) => new() { Id = character.Id, Name = character.Name, Class = character.Class };
}
