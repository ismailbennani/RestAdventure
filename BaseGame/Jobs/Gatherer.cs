using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;

namespace BaseGame.Jobs;

public class Gatherer
{
    public Gatherer()
    {
        Apple = new Item { Name = "Apple", Description = "A delicious apple.", Weight = 1 };
        Pear = new Item { Name = "Pear", Description = "A very delicious pear.", Weight = 1 };

        AppleTree = new StaticObject { Name = "Apple Tree" };
        PearTree = new StaticObject { Name = "Pear Tree" };

        PickApples = new JobHarvest
        {
            Name = "pick-apple",
            Level = 1,
            HarvestDuration = 5,
            Targets = [AppleTree],
            Items = [new ItemStack(Apple, 1)],
            Experience = 1
        };

        PickPears = new JobHarvest
        {
            Name = "pick-pear",
            Level = 2,
            HarvestDuration = 10,
            Targets = [PearTree],
            Items = [new ItemStack(Pear, 1)],
            Experience = 5
        };

        Job = new Job { Name = "gatherer", Description = "Pick stuff here and there", Innate = true, LevelCaps = [2, 5, 10], Harvests = [PickApples, PickPears] };
    }


    public Job Job { get; }
    public Item Apple { get; }
    public Item Pear { get; }
    public StaticObject AppleTree { get; }
    public StaticObject PearTree { get; }
    public JobHarvest PickApples { get; }
    public JobHarvest PickPears { get; }
}
