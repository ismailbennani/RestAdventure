using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Monsters;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Monsters;

/// <summary>
///     Monster minimal
/// </summary>
public class MonsterMinimalDto : EntityMinimalDto
{
    /// <summary>
    ///     The level of the monster
    /// </summary>
    [Required]
    public required int Level { get; init; }
}

static class MonsterMinimalMappingExtensions
{
    public static MonsterMinimalDto ToDto(this MonsterInstance monster) =>
        new()
        {
            Id = monster.Id.Guid,
            Name = monster.Name,
            Level = monster.Level
        };
}
