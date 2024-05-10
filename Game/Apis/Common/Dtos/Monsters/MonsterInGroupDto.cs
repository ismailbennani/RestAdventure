using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Entities.Monsters;

namespace RestAdventure.Game.Apis.Common.Dtos.Monsters;

/// <summary>
///     Monster in group
/// </summary>
public class MonsterInGroupDto
{
    /// <summary>
    ///     The species of the monster
    /// </summary>
    [Required]
    public required MonsterSpeciesDto Species { get; init; }

    /// <summary>
    ///     The level of the monster
    /// </summary>
    [Required]
    public int Level { get; init; }
}

static class MonsterInGroupMappingExtensions
{
    public static MonsterInGroupDto ToDto(this MonsterInGroup group) =>
        new()
        {
            Species = group.Species.ToDto(),
            Level = group.Level
        };
}
