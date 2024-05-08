using System.ComponentModel.DataAnnotations;

namespace RestAdventure.Game.Apis.Common.Dtos.Monsters;

/// <summary>
///     Monster group
/// </summary>
public class MonsterGroupDto
{
    /// <summary>
    ///     The unique ID of the group
    /// </summary>
    [Required]
    public required Guid Id { get; init; }

    /// <summary>
    ///     The monsters in the group
    /// </summary>
    [Required]
    public required IReadOnlyCollection<MonsterInstanceMinimalDto> Monsters { get; init; }

    /// <summary>
    ///     Can the character attack the monsters
    /// </summary>
    [Required]
    public required bool CanAttack { get; init; }

    /// <summary>
    ///     Why cannot the character attack the monsters
    /// </summary>
    public string? WhyCannotAttack { get; init; }

    /// <summary>
    ///     The expected experience gain if the character defeats the monster group
    /// </summary>
    [Required]
    public required int ExpectedExperience { get; init; }
}
