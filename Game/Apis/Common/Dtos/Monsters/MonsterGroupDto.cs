using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.Common.Dtos.Monsters;

/// <summary>
///     Monster group
/// </summary>
public class MonsterGroupDto : MonsterGroupMinimalDto
{
    /// <summary>
    ///     Can the character attack the monsters
    /// </summary>
    [Required]
    public required bool CanAttack { get; init; }

    /// <summary>
    ///     Why cannot the character attack the monsters
    /// </summary>
    public string? WhyCannotAttack { get; init; }
}
