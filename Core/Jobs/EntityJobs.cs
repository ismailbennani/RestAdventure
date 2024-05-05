using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Jobs;

public class EntityJobs
{
    readonly Entity _entity;
    readonly Dictionary<JobId, JobInstance> _jobs = new();

    public EntityJobs(Entity entity)
    {
        _entity = entity;
    }

    public event EventHandler<Job>? JobLearned;
    public event EventHandler<EntityJobLeveledUpEvent>? JobLeveldUp;

    public JobInstance Learn(Job job)
    {
        JobInstance jobInstance = new(job);
        _jobs[job.Id] = jobInstance;

        JobLearned?.Invoke(this, job);

        jobInstance.LeveledUp += (_, args) => JobLeveldUp?.Invoke(
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

public class EntityJobLeveledUpEvent
{
    public required Job Job { get; init; }
    public required int OldLevel { get; init; }
    public required int NewLevel { get; init; }
}
