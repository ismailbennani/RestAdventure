namespace RestAdventure.Core.Jobs;

public class GameJobs
{
    readonly Dictionary<JobId, Job> _jobs = [];

    public IEnumerable<Job> All => _jobs.Values;

    public void Register(Job job) => _jobs[job.Id] = job;
    public Job? GetJob(JobId jobId) => _jobs.GetValueOrDefault(jobId);
}

public static class GameJobsExtensions
{
    public static Job RequireJob(this GameJobs jobs, JobId jobId) => jobs.GetJob(jobId) ?? throw new InvalidOperationException($"Could not find job {jobId}");
}
