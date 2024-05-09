using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Game.Apis.Common.Dtos.Entities;

namespace RestAdventure.Game.Apis.Common.Dtos.Monsters;

/// <summary>
///     Monster minimal
/// </summary>
public class MonsterInstanceMinimalDto : EntityMinimalDto
{
    /// <summary>
    ///     The level of the monster
    /// </summary>
    [Required]
    public required int Level { get; init; }
}

static class MonsterMinimalMappingExtensions
{
    public static MonsterInstanceMinimalDto ToDto(this MonsterInstance monster) =>
        new()
        {
            Id = monster.Id.Guid,
            Name = monster.Name,
            Level = monster.Level
        };
}
