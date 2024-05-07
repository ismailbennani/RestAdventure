using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Interactions;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Interactions;

/// <summary>
///     Interaction instance
/// </summary>
public class InteractionInstanceDto
{
    /// <summary>
    ///     The unique ID of the instance
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The interaction associated with the instance
    /// </summary>
    [Required]
    public required InteractionMinimalDto Interaction { get; init; }

    /// <summary>
    ///     The target of the interaction
    /// </summary>
    [Required]
    public required EntityMinimalDto Target { get; init; }
}

static class InteractionInstanceMappingExtensions
{
    public static InteractionInstanceDto ToDto(this InteractionInstance instance) =>
        new()
        {
            Id = instance.Id.Guid,
            Interaction = instance.Interaction.ToMinimalDto(),
            Target = instance.Target.ToMinimalDto()
        };
}
