using RestAdventure.Core.Utils;

namespace RestAdventure.Core.Jobs;

public interface IJobInstance
{
    /// <summary>
    ///     The job that is instantiated.
    /// </summary>
    Job Job { get; }

    /// <summary>
    ///     The progression of the job
    /// </summary>
    IProgressionBar Progression { get; }
}
