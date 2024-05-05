using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Gameplay.Interactions;

namespace RestAdventure.Game.Apis.Common.Dtos.Interactions;

/// <summary>
///     Interaction
/// </summary>
public class InteractionDto : InteractionMinimalDto
{
    /// <summary>
    ///     Can this interaction be performed
    /// </summary>
    [Required]
    public required bool CanInteract { get; init; }
}

static class InteractionMappingExtensions
{
    public static InteractionDto ToDto(this Interaction interaction, bool canInteract) =>
        new()
        {
            Id = interaction.Id.Guid,
            Name = interaction.Name,
            CanInteract = canInteract
        };
}
