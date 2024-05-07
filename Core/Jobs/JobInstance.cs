using RestAdventure.Core.Utils;

namespace RestAdventure.Core.Jobs;

public class JobInstance
{
    public JobInstance(Job job)
    {
        Job = job;
        Progression = new ProgressionBar(job.LevelCaps);
    }

    public IEnumerable<JobHarvest> Harvests => Job.Harvests.Where(h => h.Level <= Progression.Level);

    /// <summary>
    ///     The job that is instantiated.
    /// </summary>
    public Job Job { get; }

    /// <summary>
    ///     The progression of the job
    /// </summary>
    public ProgressionBar Progression { get; }

    public override string ToString() => $"{Job}[lv. {Progression.Level}, {Progression.Experience}xp]";
}
