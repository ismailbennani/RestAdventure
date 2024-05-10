using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Entities.Monsters;

namespace RestAdventure.Game.Apis.Common.Dtos.Monsters;

/// <summary>
///     Monster species
/// </summary>
public class MonsterSpeciesDto
{
    /// <summary>
    ///     The family of monsters
    /// </summary>
    [Required]
    public required MonsterFamilyDto Family { get; init; }

    /// <summary>
    ///     The name of the species
    /// </summary>
    [Required]
    public required string Name { get; init; }

    /// <summary>
    ///     The description of the species
    /// </summary>
    public string? Description { get; init; }
}

static class MonsterSpeciesMappingExtensions
{
    public static MonsterSpeciesDto ToDto(this MonsterSpecies species) =>
        new()
        {
            Family = species.Family.ToDto(),
            Name = species.Name,
            Description = species.Description
        };
}
