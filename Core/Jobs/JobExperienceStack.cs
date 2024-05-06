namespace RestAdventure.Core.Jobs;

public class JobExperienceStack
{
    public JobExperienceStack(Job job, int amount)
    {
        Job = job;
        Amount = amount;
    }

    /// <summary>
    ///     The job for associated with this stack
    /// </summary>
    public Job Job { get; }

    /// <summary>
    ///     The amount of experience in the stack
    /// </summary>
    public int Amount { get; }
}
