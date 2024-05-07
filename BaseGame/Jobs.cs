using System.Reflection;
using RestAdventure.Core.Jobs;

namespace BaseGame;

public class Jobs
{
    public Jobs()
    {
        Gatherer = new Job { Name = "gatherer", Description = "Gather stuff", Innate = true, LevelCaps = [2, 5, 10] };
    }

    public Job Gatherer { get; }

    public IEnumerable<Job> All =>
        typeof(Jobs).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType == typeof(Job)).Select(p => p.GetValue(this)).OfType<Job>();
}
