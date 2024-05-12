namespace RestAdventure.Core.Utils;

public interface IProgressionBar
{
    int Level { get; }
    int Experience { get; }

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
    IReadOnlyList<int> LevelCaps { get; }

    int? NextLevelExperience { get; }
}
