using System.Reflection;
using BaseGame.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;

namespace BaseGame.Jobs;

public class Gatherer
{
    public Gatherer(Trees trees, Items items)
    {
        PickApples = new JobHarvest
        {
            Name = "pick-apple",
            Level = 1,
            HarvestDuration = 5,
            Targets = [trees.AppleTree],
            Items = [new ItemStack(items.Apple, 1)],
            Experience = 1
        };

        PickPears = new JobHarvest
        {
            Name = "pick-pear",
            Level = 2,
            HarvestDuration = 10,
            Targets = [trees.PearTree],
            Items = [new ItemStack(items.Pear, 1)],
            Experience = 5
        };

        Job = new Job { Name = "Gatherer", Description = "Pick stuff here and there", Innate = true, LevelCaps = [2, 5, 10], Harvests = [PickApples, PickPears] };
    }

    public Job Job { get; }
    public JobHarvest PickApples { get; }
    public JobHarvest PickPears { get; }

    public IEnumerable<Job> All =>
        typeof(Gatherer).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType == typeof(Job)).Select(p => p.GetValue(this)).OfType<Job>();
}
