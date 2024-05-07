using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Interactions;

namespace RestAdventure.Game.Apis.Common.Dtos.Interactions;

/// <summary>
///     Interaction (minimal)
/// </summary>
public class InteractionMinimalDto
{
    /// <summary>
    ///     The name of the interaction
    /// </summary>
    [Required]
    public required string Name { get; init; }
}

static class InteractionMinimalMappingExtensions
{
    public static InteractionMinimalDto ToMinimalDto(this Interaction interaction) => new() { Name = interaction.Name };
}
