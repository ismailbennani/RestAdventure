using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using SandboxGame.Experience;

namespace SandboxGame.Jobs;

public class Miner
{
    public Miner()
    {
        OreCategory = new ItemCategory
        {
            Name = "Ore"
        };

        Iron = new Item
        {
            Name = "Iron",
            Description = "A common metallic ore used extensively in crafting tools, weapons, and armor due to its durability and versatility.",
            Category = OreCategory,
            Weight = 10
        };

        Copper = new Item
        {
            Name = "Copper",
            Description = "Known for its malleability and conductivity, widely used in electrical wiring, plumbing, and as an alloy in metalworking.",
            Category = OreCategory,
            Weight = 10
        };

        Silver = new Item
        {
            Name = "Silver",
            Description = "Valued for its luster and conductivity, used in various applications including jewelry, electronics, and as a currency in some cultures.",
            Category = OreCategory,
            Weight = 10
        };

        Gold = new Item
        {
            Name = "Gold",
            Description = "Highly sought after for its intrinsic value and decorative properties, often used in jewelry, currency, and as a symbol of wealth.",
            Category = OreCategory,
            Weight = 10
        };

        Mithril = new Item
        {
            Name = "Mithril",
            Description =
                "A rare and valuable ore prized for its lightweight yet durable properties, often used in the crafting of high-quality weapons and armor in fantasy settings.",
            Category = OreCategory,
            Weight = 10
        };

        IronOre = new StaticObject { Name = "Iron Ore" };
        CopperOre = new StaticObject { Name = "Copper Ore" };
        SilverOre = new StaticObject { Name = "Silver Ore" };
        GoldOre = new StaticObject { Name = "Gold Ore" };
        MithrilOre = new StaticObject { Name = "Mithril Ore" };

        MineIron = new JobHarvest
        {
            Name = "mine-iron",
            Level = 1,
            HarvestDuration = 2,
            Targets = [IronOre],
            Tool = null,
            Items = [new ItemStack(Iron, 1)],
            Experience = 1
        };

        MineCopper = new JobHarvest
        {
            Name = "mine-copper",
            Level = 10,
            HarvestDuration = 2,
            Targets = [CopperOre],
            Tool = null,
            Items = [new ItemStack(Copper, 1)],
            Experience = 5
        };

        MineSilver = new JobHarvest
        {
            Name = "mine-silver",
            Level = 20,
            HarvestDuration = 2,
            Targets = [SilverOre],
            Tool = null,
            Items = [new ItemStack(Silver, 1)],
            Experience = 15
        };

        MineGold = new JobHarvest
        {
            Name = "mine-gold",
            Level = 30,
            HarvestDuration = 2,
            Targets = [GoldOre],
            Tool = null,
            Items = [new ItemStack(Gold, 1)],
            Experience = 30
        };

        MineMithril = new JobHarvest
        {
            Name = "mine-mithril",
            Level = 40,
            HarvestDuration = 2,
            Targets = [MithrilOre],
            Tool = null,
            Items = [new ItemStack(Mithril, 1)],
            Experience = 50
        };

        Job = new Job
        {
            Name = "miner", Description = "Miners mine ores.", Innate = true, LevelCaps = KnownExperienceFormulas.JobLevelCaps1To50,
            Harvests = [MineCopper, MineSilver, MineGold, MineMithril, MineIron]
        };
    }

    public Job Job { get; }
    public ItemCategory OreCategory { get; }
    public Item Copper { get; }
    public Item Silver { get; }
    public Item Gold { get; set; }
    public Item Mithril { get; set; }
    public Item Iron { get; set; }
    public StaticObject IronOre { get; }
    public StaticObject CopperOre { get; }
    public StaticObject SilverOre { get; }
    public StaticObject GoldOre { get; }
    public StaticObject MithrilOre { get; }
    public JobHarvest MineCopper { get; }
    public JobHarvest MineSilver { get; }
    public JobHarvest MineGold { get; }
    public JobHarvest MineMithril { get; }
    public JobHarvest MineIron { get; }
}
