using RestAdventure.Core.Jobs;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Core.Utils;

namespace RestAdventure.Core.Serialization.Jobs;

public class JobInstanceSnapshot : IJobInstance
{
    public required Job Job { get; init; }
    public required ProgressionBarSnapshot Progression { get; init; }
    IProgressionBar IJobInstance.Progression => Progression;

    public static JobInstanceSnapshot Take(JobInstance instance) =>
        new()
        {
            Job = instance.Job,
            Progression = ProgressionBarSnapshot.Take(instance.Progression)
        };
}
