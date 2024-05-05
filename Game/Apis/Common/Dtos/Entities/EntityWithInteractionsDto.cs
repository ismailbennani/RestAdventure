using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Game.Apis.Common.Dtos.Interactions;

namespace RestAdventure.Game.Apis.Common.Dtos.Entities;

/// <summary>
///     Entity with interactions
/// </summary>
public class EntityWithInteractionsDto : EntityMinimalDto
{
    /// <summary>
    ///     The interactions that can be performed on the entity
    /// </summary>
    [Required]
    public required IReadOnlyCollection<InteractionDto> Interactions { get; init; }
}

static class EntityWithInteractionsMappingExtensions
{
    public static EntityWithInteractionsDto ToEntityWithInteractionsDto(this IGameEntityWithInteractions entity, Character character) =>
        new()
        {
            Id = entity.Id.Guid,
            Name = entity.Name,
            Interactions = entity.Interactions.All.Select(i => i.ToDto(i.CanInteract(character, entity))).ToArray()
        };
}
