namespace RestAdventure.Core.Jobs;

public class EntityJobs
{
    readonly Dictionary<JobId, JobInstance> _jobs = new();

    public IEnumerable<JobInstance> All => _jobs.Values.Where(j => j.Level > 0);
    public event EventHandler<Job>? JobLearned;
    public event EventHandler<EntityJobGainedExperienceEvent>? JobGainedExperience;
    public event EventHandler<EntityJobLeveledUpEvent>? JobLeveledUp;

    public JobInstance Learn(Job job)
    {
        JobInstance jobInstance = new(job);
        _jobs[job.Id] = jobInstance;

        JobLearned?.Invoke(this, job);

        jobInstance.GainedExperience += (_, args) => JobGainedExperience?.Invoke(
            this,
            new EntityJobGainedExperienceEvent
            {
                Job = jobInstance.Job,
                OldExperience = args.OldExperience,
                NewExperience = args.NewExperience
            }
        );

        jobInstance.LeveledUp += (_, args) => JobLeveledUp?.Invoke(
            this,
            new EntityJobLeveledUpEvent
            {
                Job = jobInstance.Job,
                OldLevel = args.OldLevel,
                NewLevel = args.NewLevel
            }
        );

        return jobInstance;
    }

    public JobInstance? Get(Job job) => _jobs.GetValueOrDefault(job.Id);
}

public class EntityJobGainedExperienceEvent
{
    public required Job Job { get; init; }
    public required int OldExperience { get; init; }
    public required int NewExperience { get; init; }
}

public class EntityJobLeveledUpEvent
{
    public required Job Job { get; init; }
    public required int OldLevel { get; init; }
    public required int NewLevel { get; init; }
}

public static class EntityJobsExtensions
{
    public static void GainExperience(this EntityJobs jobs, IEnumerable<JobExperienceStack> stacks)
    {
        foreach (JobExperienceStack stack in stacks)
        {
            JobInstance? instance = jobs.Get(stack.Job);
            instance?.GainExperience(stack.Amount);
        }
    }
}
