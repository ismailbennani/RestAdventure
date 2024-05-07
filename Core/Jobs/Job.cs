using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Jobs;

public record JobId(Guid Guid) : ResourceId(Guid);

/// <summary>
///     A job.
///     There should be only one instance of this class per item. It stores all the meta data about an item: its name, description, etc...
///     The materialization of an item in the game world is <see cref="JobInstance" />.
/// </summary>
public class Job : GameResource<JobId>
{
    public Job() : base(new JobId(Guid.NewGuid())) { }

    /// <summary>
    ///     The name of the job
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The description of the job
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     Is the job learned automatically by new characters
    /// </summary>
    public bool Innate { get; init; } = false;

    /// <summary>
    ///     The experience to reach each level of the job.
    ///     This list of values is strictly ascending, the experience to reach the <c>N</c>th level of the job is stored in cell <c>N-2</c>.
    /// </summary>
    /// <example>
    ///     The list <c>[12, 46, 98]</c> should be understood as:
    ///     <list type="bullet">
    ///         <item>Level 2 requires 12 experience points (cell 0)</item>
    ///         <item>Level 3 requires 46 experience points (cell 1)</item>
    ///         <item>Level 4 requires 98 experience points (cell 2)</item>
    ///         <item>Level 4 is the max level</item>
    ///     </list>
    /// </example>
    public IReadOnlyList<int> LevelCaps { get; init; } = Array.Empty<int>();

    /// <summary>
    ///     The harvests enabled by the job
    /// </summary>
    public IReadOnlyCollection<JobHarvest> Harvests = Array.Empty<JobHarvest>();

    public override string ToString() => $"Job {Name} ({Id})";
}
