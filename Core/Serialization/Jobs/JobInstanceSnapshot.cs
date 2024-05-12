using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Serialization.Entities;

public class JobInstanceSnapshot
{
    public required Job Job { get; init; }
    public required ProgressionBarSnapshot Progression { get; init; }

    public static JobInstanceSnapshot Take(JobInstance instance) =>
        new()
        {
            Job = instance.Job,
            Progression = ProgressionBarSnapshot.Take(instance.Progression)
        };
}