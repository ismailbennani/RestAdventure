using System.Reflection;
using RestAdventure.Core.Conditions.Characters;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Harvestables;

namespace BaseGame;

public class Harvestables
{
    public Harvestables(Items items, Jobs jobs)
    {
        AppleTree = new Harvestable
        {
            Name = "Apple Tree",
            Description = "A tree that has apples.",
            HarvestCondition = new CharacterJobCondition(jobs.Gatherer),
            HarvestDuration = 10,
            Items = [new ItemStack(items.Apple, 1)],
            Experience = [new JobExperienceStack(jobs.Gatherer, 1)]
        };

        PearTree = new Harvestable
        {
            Name = "Pear Tree",
            Description = "A tree that has pears.",
            HarvestCondition = new CharacterJobCondition(jobs.Gatherer, 2),
            HarvestDuration = 10,
            Items = [new ItemStack(items.Pear, 1)],
            Experience = [new JobExperienceStack(jobs.Gatherer, 5)]
        };
    }

    public Harvestable AppleTree { get; }
    public Harvestable PearTree { get; }

    public IEnumerable<Harvestable> All =>
        typeof(Harvestables).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.PropertyType == typeof(Harvestable))
            .Select(p => p.GetValue(this))
            .OfType<Harvestable>();
}
