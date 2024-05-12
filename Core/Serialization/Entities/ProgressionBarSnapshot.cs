using RestAdventure.Core.Utils;

namespace RestAdventure.Core.Serialization.Entities;

public class ProgressionBarSnapshot : IProgressionBar
{
    public required int Level { get; init; }
    public required int Experience { get; init; }
    public required IReadOnlyList<int> LevelCaps { get; init; }
    public int? NextLevelExperience => LevelCaps.Count > Level - 1 ? LevelCaps[Level - 1] : null;

    public static ProgressionBarSnapshot Take(ProgressionBar bar) =>
        new()
        {
            Level = bar.Level,
            Experience = bar.Experience,
            LevelCaps = bar.LevelCaps.ToList()
        };
}
