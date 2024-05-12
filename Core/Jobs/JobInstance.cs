using RestAdventure.Core.Utils;

namespace RestAdventure.Core.Jobs;

public class JobInstance : IJobInstance
{
    public JobInstance(Job job)
    {
        Job = job;
        Progression = new ProgressionBar(job.LevelCaps);
    }

    public Job Job { get; }
    public ProgressionBar Progression { get; }
    IProgressionBar IJobInstance.Progression => Progression;

    public override string ToString() => $"{Job}[lv. {Progression.Level}, {Progression.Experience}xp]";
}
