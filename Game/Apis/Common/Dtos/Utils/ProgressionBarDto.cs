﻿using System.ComponentModel.DataAnnotations;
using RestAdventure.Core.Utils;

namespace RestAdventure.Game.Apis.Common.Dtos.Utils;

/// <summary>
///     Progression bar
/// </summary>
public class ProgressionBarDto : ProgressionBarMinimalDto
{
    /// <summary>
    ///     The level caps
    /// </summary>
    [Required]
    public required IReadOnlyCollection<int> LevelCaps { get; init; }
}

static class ProgressionBarMappingExtensions
{
    public static ProgressionBarDto ToDto(this IProgressionBar bar) =>
        new()
        {
            Level = bar.Level,
            Experience = bar.Experience,
            NextLevelExperience = bar.NextLevelExperience,
            LevelCaps = bar.LevelCaps
        };
}
