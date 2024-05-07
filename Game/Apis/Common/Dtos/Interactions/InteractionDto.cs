using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Kernel.Errors;

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

    /// <summary>
    ///     Why cannot this interaction be performed
    /// </summary>
    public string? WhyNot { get; init; }
}

static class InteractionMappingExtensions
{
    public static InteractionDto ToDto(this Interaction interaction, Maybe canInteract) =>
        new()
        {
            Id = interaction.Id.Guid,
            Name = interaction.Name,
            CanInteract = canInteract.Success,
            WhyNot = canInteract.WhyNot
        };
}
