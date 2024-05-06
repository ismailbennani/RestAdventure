using RestAdventure.Core.Jobs;

namespace BaseGame;

public class Jobs
{
    public Jobs()
    {
        Gatherer = new Job { Name = "Gatherer", Description = "Gather stuff", Innate = true, LevelCaps = [2, 5, 10] };
    }

    public Job Gatherer { get; }
}
