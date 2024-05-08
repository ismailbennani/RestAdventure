using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Maps;
using RestAdventure.Game.Apis.Common.Dtos.Maps;

namespace RestAdventure.Game.Apis.Common.Dtos.Characters.Actions;

/// <summary>
///     Move action
/// </summary>
public class MoveActionDto : ActionDto
{
    /// <summary>
    ///     The location to which the character is moving
    /// </summary>
    [Required]
    public required LocationMinimalDto Location { get; init; }
}

static class MoveActionMappingExtensions
{
    public static MoveActionDto ToDto(this MoveAction action) => new() { Name = action.Name, Location = action.Location.ToMinimalDto() };
}
