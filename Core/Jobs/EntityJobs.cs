using System.Collections;
using RestAdventure.Core.StaticObjects;

namespace RestAdventure.Core.Jobs;

public class EntityJobs : IReadOnlyCollection<JobInstance>, IDisposable
{
    readonly Dictionary<JobId, JobInstance> _jobs = new();

    public int Count => _jobs.Count;
    public IEnumerator<JobInstance> GetEnumerator() => _jobs.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_jobs).GetEnumerator();

    public event EventHandler<Job>? JobLearned;
    public event EventHandler<EntityJobGainedExperienceEvent>? JobGainedExperience;
    public event EventHandler<EntityJobLeveledUpEvent>? JobLeveledUp;

    public JobInstance Learn(Job job)
    {
        JobInstance jobInstance = new(job);
        _jobs[job.Id] = jobInstance;

        JobLearned?.Invoke(this, job);

        jobInstance.Progression.Progressed += (_, args) => JobGainedExperience?.Invoke(
            this,
            new EntityJobGainedExperienceEvent
            {
                Job = jobInstance.Job,
                OldExperience = args.OldExperience,
                NewExperience = args.NewExperience
            }
        );

        jobInstance.Progression.LeveledUp += (_, args) => JobLeveledUp?.Invoke(
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

    public IEnumerable<JobHarvest> GetAvailableHarvests(StaticObjectInstance staticObject)
    {
        foreach (JobInstance job in _jobs.Values)
        {
            foreach (JobHarvest harvest in job.Harvests)
            {
                if (job.Progression.Level >= harvest.Level && harvest.Match(staticObject))
                {
                    yield return harvest;
                }
            }
        }
    }

    public void Dispose()
    {
        JobGainedExperience = null;
        JobLeveledUp = null;
        GC.SuppressFinalize(this);
    }
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
