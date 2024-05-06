using System.ComponentModel.DataAnnotations;
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
}

static class ProgressionBarMinimalMappingExtensions
{
    public static ProgressionBarMinimalDto ToMinimalDto(this ProgressionBar bar) =>
        new()
        {
            Level = bar.Level,
            Experience = bar.Experience
        };
}
