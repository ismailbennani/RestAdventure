using System.ComponentModel.DataAnnotations;
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
