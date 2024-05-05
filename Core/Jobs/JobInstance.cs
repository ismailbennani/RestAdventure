namespace RestAdventure.Core.Jobs;

public class JobInstance
{
    public JobInstance(Job job)
    {
        Job = job;
    }

    /// <summary>
    ///     The job that is instantiated.
    /// </summary>
    public Job Job { get; }

    /// <summary>
    ///     The current level of the job.
    ///     This value is strictly positive.
    /// </summary>
    public int Level { get; private set; } = 1;

    /// <summary>
    ///     The current experience points acquired.
    ///     This value is positive or zero.
    /// </summary>
    public int Experience { get; private set; }

    /// <summary>
    ///     Event fired each time the job levels up.
    /// </summary>
    public event EventHandler<JobLeveledUpEvent>? LeveledUp;

    public void GainExperience(int experience)
    {
        if (experience < 0)
        {
            throw new ArgumentException($"Expected experience to be positive, but got {experience}.");
        }

        Experience += experience;
        if (Job.LevelsExperience.Any())
        {
            int oldLevel = Level;
            Level = Job.LevelsExperience.Last(levelExperience => Experience > levelExperience) + 2;

            if (Level > oldLevel)
            {
                LeveledUp?.Invoke(this, new JobLeveledUpEvent { OldLevel = oldLevel, NewLevel = Level });
            }
        }
    }
}

public class JobLeveledUpEvent
{
    public required int OldLevel { get; init; }
    public required int NewLevel { get; init; }
}
