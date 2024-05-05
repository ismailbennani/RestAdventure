namespace RestAdventure.Core.Jobs;

public interface IEntityWithJobs
{
    EntityJobs Jobs { get; }
}
