using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Gameplay.Interactions;

namespace RestAdventure.Game.Apis.Common.Dtos.Interactions;

/// <summary>
///     Interaction
/// </summary>
public class InteractionDto
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

static class InteractionMappingExtensions
{
    public static InteractionDto ToDto(this Interaction interaction) => new() { Id = interaction.Id.Guid, Name = interaction.Name };
}
