using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Entities.Monsters;

namespace RestAdventure.Game.Apis.Common.Dtos.Monsters;

/// <summary>
///     Monster family
/// </summary>
public class MonsterFamilyDto
{
    /// <summary>
    ///     The name of the family
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     The description of the family
    /// </summary>
    public string? Description { get; init; }
}

static class MonsterFamilyMappingExtensions
{
    public static MonsterFamilyDto ToDto(this MonsterFamily family) =>
        new()
        {
            Name = family.Name,
            Description = family.Description
        };
}
