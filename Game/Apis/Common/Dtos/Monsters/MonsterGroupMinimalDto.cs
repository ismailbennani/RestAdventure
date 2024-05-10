using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Entities.Monsters;

namespace RestAdventure.Game.Apis.Common.Dtos.Monsters;

/// <summary>
///     Monster group (minimal)
/// </summary>
public class MonsterGroupMinimalDto
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
    public required IReadOnlyCollection<MonsterInGroupDto> Monsters { get; init; }

    /// <summary>
    ///     The expected experience gain if the character defeats the monster group
    /// </summary>
    [Required]
    public required int ExpectedExperience { get; init; }
}

static class MonsterGroupMinimalMappingExtensions
{
    public static MonsterGroupMinimalDto ToMonsterGroupMinimalDto(this MonsterGroup group) =>
        new()
        {
            Id = group.Id.Guid,
            Monsters = group.Monsters.Select(m => m.ToDto()).ToArray(),
            ExpectedExperience = group.Monsters.Sum(m => m.Species.Experience)
        };
}
