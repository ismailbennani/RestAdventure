﻿using System.ComponentModel.DataAnnotations;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.Maps;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters;

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
    public required LocationDto Location { get; init; }

    /// <summary>
    ///     The inventory of the character
    /// </summary>
    [Required]
    public required InventoryDto Inventory { get; init; }

    /// <summary>
    ///     The result of the action that has been performed on last tick
    /// </summary>
    public CharacterActionResultDto? LastActionResult { get; init; }

    /// <summary>
    ///     The action that the character has planned for the next tick
    /// </summary>
    public CharacterActionDto? NextAction { get; init; }
}

static class TeamCharacterMappingExtensions
{
    public static TeamCharacterDto ToDto(this Character character, GameContent content, CharacterMappingOptions? options = null) =>
        new()
        {
            Id = character.Id.Guid,
            Name = character.Name,
            Class = character.Class,
            Location = character.Location.ToDiscoveredLocationDto(content),
            Inventory = character.Inventory.ToDto(),
            LastActionResult = options?.LastActionResult?.ToDto(),
            NextAction = options?.NextAction?.ToDto()
        };
}

class CharacterMappingOptions
{
    public CharacterActionResult? LastActionResult { get; init; }
    public CharacterAction? NextAction { get; init; }
}
