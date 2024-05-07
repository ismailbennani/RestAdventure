using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Jobs;

public class GameJobs : GameResourcesStore<JobId, Job>
{
    public IEnumerable<Job> Innate => this.Where(j => j.Innate);
}
