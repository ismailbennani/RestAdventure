﻿using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.Jobs;
using RestAdventure.Game.Apis.Common.Dtos.Maps;
using RestAdventure.Game.Apis.Common.Dtos.Utils;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters;

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
    public required CharacterClassMinimalDto Class { get; init; }

    /// <summary>
    ///     The health of the character
    /// </summary>
    [Required]
    public int Health { get; init; }

    /// <summary>
    ///     The health of the character
    /// </summary>
    [Required]
    public int MaxHealth { get; init; }

    /// <summary>
    ///     The progression of the character
    /// </summary>
    [Required]
    public required ProgressionBarMinimalDto Progression { get; init; }

    /// <summary>
    ///     The current location of the character
    /// </summary>
    [Required]
    public required LocationMinimalDto Location { get; init; }

    /// <summary>
    ///     The inventory of the character
    /// </summary>
    [Required]
    public required InventoryDto Inventory { get; init; }

    /// <summary>
    ///     The jobs of the character
    /// </summary>
    [Required]
    public required IReadOnlyCollection<JobInstanceDto> Jobs { get; init; }

    /// <summary>
    ///     The interaction being performed by the character
    /// </summary>
    public ActionDto? OngoingAction { get; init; }

    /// <summary>
    ///     The action that the character has planned for the next tick
    /// </summary>
    public ActionDto? PlannedAction { get; init; }
}

static class TeamCharacterMappingExtensions
{
    public static CharacterDto ToDto(this Character character, CharacterMappingOptions? options = null) =>
        new()
        {
            Id = character.Id.Guid,
            Name = character.Name,
            Class = character.Class.ToMinimalDto(),
            Health = character.Health,
            MaxHealth = character.Class.Health,
            Progression = character.Progression.ToMinimalDto(),
            Location = character.Location.ToMinimalDto(),
            Inventory = character.Inventory.ToDto(),
            Jobs = character.Jobs.Select(j => j.ToDto()).ToArray(),
            OngoingAction = options?.OngoingAction?.ToDto(),
            PlannedAction = options?.PlannedAction?.ToDto()
        };

    public static CharacterDto ToDto(this CharacterSnapshot character, CharacterMappingOptions? options = null) =>
        new()
        {
            Id = character.Id.Guid,
            Name = character.Name,
            Class = character.Class.ToMinimalDto(),
            Health = character.Health,
            MaxHealth = character.Class.Health,
            Progression = character.Progression.ToMinimalDto(),
            Location = character.Location.ToMinimalDto(),
            Inventory = character.Inventory.ToDto(),
            Jobs = character.Jobs.Select(j => j.ToDto()).ToArray(),
            OngoingAction = options?.OngoingAction?.ToDto(),
            PlannedAction = options?.PlannedAction?.ToDto()
        };
}

class CharacterMappingOptions
{
    public Action? OngoingAction { get; init; }
    public Action? PlannedAction { get; init; }
}
