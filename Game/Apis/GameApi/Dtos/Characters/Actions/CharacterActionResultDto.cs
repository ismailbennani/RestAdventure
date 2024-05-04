using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Gameplay.Actions;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Characters.Actions;

/// <summary>
///     The result of an action performed by a character
/// </summary>
public class CharacterActionResultDto
{
    /// <summary>
    ///     The tick at which the action has been performed
    /// </summary>
    [Required]
    public required long Tick { get; init; }

    /// <summary>
    ///     The action
    /// </summary>
    [Required]
    public required CharacterActionDto Action { get; init; }

    /// <summary>
    ///     Has the action been successful
    /// </summary>
    [Required]
    public required bool Success { get; init; }

    /// <summary>
    ///     Why the action has failed
    /// </summary>
    public string? FailureReason { get; init; }
}

static class CharacterActionResultMappingExtensions
{
    public static CharacterActionResultDto ToDto(this CharacterActionResult result) =>
        new()
        {
            Tick = result.Tick, Action = result.Action.ToDto(), Success = result.Success, FailureReason = result.FailureReason
        };
}
