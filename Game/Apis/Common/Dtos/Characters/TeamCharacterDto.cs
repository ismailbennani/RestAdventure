using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Actions;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;
using RestAdventure.Game.Apis.Common.Dtos.Interactions;
using RestAdventure.Game.Apis.Common.Dtos.Items;
using RestAdventure.Game.Apis.Common.Dtos.Jobs;
using RestAdventure.Game.Apis.Common.Dtos.Maps;
using RestAdventure.Game.Apis.Common.Dtos.Utils;

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
    public required CharacterClassMinimalDto Class { get; init; }

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
    ///     The result of the action that has been performed on last tick
    /// </summary>
    public CharacterActionResultDto? LastActionResult { get; init; }

    /// <summary>
    ///     The interaction being performed by the character
    /// </summary>
    public InteractionInstanceDto? CurrentInteraction { get; init; }

    /// <summary>
    ///     The action that the character has planned for the next tick
    /// </summary>
    public CharacterActionDto? PlannedAction { get; init; }
}

static class TeamCharacterMappingExtensions
{
    public static TeamCharacterDto ToDto(this Character character, CharacterMappingOptions? options = null) =>
        new()
        {
            Id = character.Id.Guid,
            Name = character.Name,
            Class = character.Class.ToMinimalDto(),
            Progression = character.Progression.ToMinimalDto(),
            Location = character.Location.ToMinimalDto(),
            Inventory = character.Inventory.ToDto(),
            Jobs = character.Jobs.All.Select(j => j.ToDto()).ToArray(),
            LastActionResult = options?.LastActionResult?.ToDto(),
            CurrentInteraction = options?.InteractionInstance?.ToDto(),
            PlannedAction = options?.NextAction?.ToDto()
        };
}

class CharacterMappingOptions
{
    public CharacterActionResult? LastActionResult { get; init; }
    public InteractionInstance? InteractionInstance { get; init; }
    public CharacterAction? NextAction { get; init; }
}
