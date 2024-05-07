namespace RestAdventure.Core.Utils;

public class ProgressionBar
{
    public ProgressionBar(IReadOnlyList<int> levelCaps) : this(1, levelCaps)
    {
    }

    public ProgressionBar(int level, IReadOnlyList<int> levelCaps)
    {
        int maxLevel = levelCaps.Count + 1;

        Level = Math.Clamp(level, 1, maxLevel);
        Experience = Level == 1 ? 0 : levelCaps[level - 2];
        LevelCaps = levelCaps;
    }

    public int Level { get; private set; }
    public int Experience { get; private set; }

    /// <summary>
    ///     The experience to reach each level of the progress bar.
    ///     This list of values is strictly ascending, the experience to reach the <c>N</c>th level of the progression is stored in cell <c>N-2</c>.
    /// </summary>
    /// <example>
    ///     The list <c>[12, 46, 98]</c> should be understood as:
    ///     <list type="bullet">
    ///         <item>Level 2 requires 12 points (cell 0)</item>
    ///         <item>Level 3 requires 46 points (cell 1)</item>
    ///         <item>Level 4 requires 98 points (cell 2)</item>
    ///         <item>Level 4 is the max level</item>
    ///     </list>
    /// </example>
    public IReadOnlyList<int> LevelCaps { get; private set; }

    public int? NextLevelExperience => LevelCaps.Count > Level - 1 ? LevelCaps[Level - 1] : null;

    public event EventHandler<ProgressedEvent>? Progressed;
    public event EventHandler<LeveledUpEvent>? LeveledUp;

    public void Progress(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException($"Expected amount to be positive, but got {amount}.");
        }

        int oldExperience = Experience;
        Experience += amount;

        Progressed?.Invoke(this, new ProgressedEvent { OldExperience = oldExperience, NewExperience = Experience });

        if (LevelCaps.Any())
        {
            int oldLevel = Level;
            Level = LevelCaps.Select((exp, index) => new { Index = index, Experience = exp })
                        .LastOrDefault(x => Experience >= x.Experience, new { Index = -1, Experience = 0 })
                        .Index
                    + 2;

            if (Level > oldLevel)
            {
                LeveledUp?.Invoke(this, new LeveledUpEvent { OldLevel = oldLevel, NewLevel = Level });
            }
        }
    }

    public class ProgressedEvent
    {
        public required int OldExperience { get; init; }
        public required int NewExperience { get; init; }
    }

    public class LeveledUpEvent
    {
        public required int OldLevel { get; init; }
        public required int NewLevel { get; init; }
    }
}
