using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.Common.Dtos.Monsters;

/// <summary>
///     Monster group
/// </summary>
public class MonsterGroupDto : MonsterGroupMinimalDto
{
    /// <summary>
    ///     Is the monster group being attacked
    /// </summary>
    [Required]
    public bool Attacked { get; init; }

    /// <summary>
    ///     Can the character attack the monsters
    /// </summary>
    [Required]
    public required bool CanAttackOrJoin { get; init; }

    /// <summary>
    ///     Why cannot the character attack the monsters
    /// </summary>
    public string? WhyCannotAttackOrJoin { get; init; }
}
