using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Interactions;
using RestAdventure.Game.Apis.Common.Dtos.Entities;
using RestAdventure.Game.Apis.Common.Dtos.Interactions;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     Character interact with entity
/// </summary>
public class CharacterInteractWithEntityActionDto : CharacterActionDto
{
    /// <summary>
    ///     The interaction
    /// </summary>
    [Required]
    public required InteractionMinimalDto Interaction { get; init; }

    /// <summary>
    ///     The subject of the interaction
    /// </summary>
    [Required]
    public required EntityMinimalDto Entity { get; init; }
}

static class CharacterInteractWithEntityActionMappingExtensions
{
    public static CharacterInteractWithEntityActionDto ToDto(this CharacterInteractWithEntityAction action) =>
        new()
        {
            Interaction = action.Interaction.ToMinimalDto(),
            Entity = action.Entity.ToMinimalDto()
        };
}
