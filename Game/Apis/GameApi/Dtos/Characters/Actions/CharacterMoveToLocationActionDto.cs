using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Gameplay.Actions;

namespace RestAdventure.Game.Apis.GameApi.Dtos.Characters.Actions;

/// <summary>
///     Character moves to location
/// </summary>
public class CharacterMoveToLocationActionDto : CharacterActionDto
{
    /// <summary>
    ///     The location to which the character is moving
    /// </summary>
    [Required]
    public required Guid LocationId { get; init; }
}

static class CharacterMoveToLocationActionMappingExtensions
{
    public static CharacterMoveToLocationActionDto ToDto(this CharacterMoveToLocationAction action) => new() { LocationId = action.LocationId.Guid };
}
