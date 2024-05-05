using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Gameplay.Interactions;

namespace RestAdventure.Game.Apis.Common.Dtos.Interactions;

/// <summary>
///     Interaction (minimal)
/// </summary>
public class InteractionMinimalDto
{
    /// <summary>
    ///     The unique ID of the interaction
    /// </summary>
    /// <returns></returns>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The name of the interaction
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class InteractionMinimalMappingExtensions
{
    public static InteractionMinimalDto ToMinimalDto(this Interaction interaction) => new() { Id = interaction.Id.Guid, Name = interaction.Name };
}
