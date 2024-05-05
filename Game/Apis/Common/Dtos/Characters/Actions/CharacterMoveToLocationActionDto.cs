﻿using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Game.Apis.Common.Dtos.Maps;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     Character moves to location
/// </summary>
public class CharacterMoveToLocationActionDto : CharacterActionDto
{
    /// <summary>
    ///     The location to which the character is moving
    /// </summary>
    [Required]
    public required MapLocationMinimalDto Location { get; init; }
}

static class CharacterMoveToLocationActionMappingExtensions
{
    public static CharacterMoveToLocationActionDto ToDto(this CharacterMoveToLocationAction action) => new() { Location = action.Location.ToMinimalDto() };
}
