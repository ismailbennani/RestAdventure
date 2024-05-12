using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Core.Utils;

namespace RestAdventure.Game.Apis.Common.Dtos.Utils;

/// <summary>
///     Progression bar (minimal)
/// </summary>
public class ProgressionBarMinimalDto
{
    /// <summary>
    ///     The level of progression
    /// </summary>
    [Required]
    public required int Level { get; init; }

    /// <summary>
    ///     The experience acquired
    /// </summary>
    [Required]
    public required int Experience { get; init; }

    /// <summary>
    ///     The experience required to reach next level. The value is null when Level is the max level.
    /// </summary>
    [Required]
    public required int? NextLevelExperience { get; init; }
}

static class ProgressionBarMinimalMappingExtensions
{
    public static ProgressionBarMinimalDto ToMinimalDto(this ProgressionBar bar) =>
        new()
        {
            Level = bar.Level,
            Experience = bar.Experience,
            NextLevelExperience = bar.NextLevelExperience
        };

    public static ProgressionBarMinimalDto ToMinimalDto(this ProgressionBarSnapshot bar) =>
        new()
        {
            Level = bar.Level,
            Experience = bar.Experience,
            NextLevelExperience = bar.NextLevelExperience
        };
}
