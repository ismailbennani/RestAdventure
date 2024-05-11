using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using SandboxGame.Experience;

namespace SandboxGame.Jobs;

public class Forester
{
    public Forester()
    {
        OakWood = new Item
        {
            Name = "Oak Wood",
            Description = "Oak wood is sturdy and commonly used in construction, furniture making, and shipbuilding. It's known for its strength and durability.",
            Weight = 10
        };

        PineWood = new Item
        {
            Name = "Pine Wood",
            Description = "Pine trees provide softwood that's often used for construction, furniture, and paper production. They are fast-growing and widely distributed.",
            Weight = 10
        };

        MapleWood = new Item
        {
            Name = "Maple Wood",
            Description = "Maple wood is prized for its beauty and versatility. It's commonly used in furniture making, flooring, and musical instruments.",
            Weight = 10
        };

        BirchWood = new Item
        {
            Name = "Birch Wood",
            Description = "Birch trees have distinctive white or silver bark and provide lightweight wood that's used for furniture, plywood, and decorative items.",
            Weight = 10
        };

        WalnutWood = new Item
        {
            Name = "Walnut Wood",
            Description =
                "Walnut trees produce hardwood with a rich, dark color and attractive grain patterns. Walnut wood is highly valued for furniture making, cabinetry, and woodworking crafts.",
            Weight = 10
        };

        OakTree = new StaticObject { Name = "Oak Tree" };
        PineTree = new StaticObject { Name = "Pine Tree" };
        MapleTree = new StaticObject { Name = "Maple Tree" };
        BirchTree = new StaticObject { Name = "Birch Tree" };
        WalnutTree = new StaticObject { Name = "Walnut Tree" };

        CutOak = new JobHarvest
        {
            Name = "cut-oak",
            Level = 1,
            HarvestDuration = 5,
            Targets = [OakTree],
            Items = [new ItemStack(OakWood, 1)],
            Experience = 1
        };

        CutPine = new JobHarvest
        {
            Name = "cut-pine",
            Level = 10,
            HarvestDuration = 5,
            Targets = [PineTree],
            Items = [new ItemStack(PineWood, 1)],
            Experience = 5
        };

        CutMaple = new JobHarvest
        {
            Name = "cut-maple",
            Level = 20,
            HarvestDuration = 15,
            Targets = [MapleTree],
            Items = [new ItemStack(MapleWood, 1)],
            Experience = 5
        };

        CutBirch = new JobHarvest
        {
            Name = "cut-Birch",
            Level = 30,
            HarvestDuration = 5,
            Targets = [BirchTree],
            Items = [new ItemStack(BirchWood, 1)],
            Experience = 30
        };

        CutWalnut = new JobHarvest
        {
            Name = "cut-pine",
            Level = 40,
            HarvestDuration = 5,
            Targets = [WalnutTree],
            Items = [new ItemStack(WalnutWood, 1)],
            Experience = 50
        };

        Job = new Job
        {
            Name = "forester", Description = "Lumberjacks cut trees.", Innate = true, LevelCaps = KnownExperienceFormulas.JobLevelCaps1To50,
            Harvests = [CutOak, CutPine, CutMaple, CutBirch, CutWalnut]
        };
    }


    public Job Job { get; }
    public Item OakWood { get; }
    public Item PineWood { get; }
    public Item MapleWood { get; set; }
    public Item BirchWood { get; set; }
    public Item WalnutWood { get; set; }
    public StaticObject OakTree { get; }
    public StaticObject PineTree { get; }
    public StaticObject MapleTree { get; }
    public StaticObject BirchTree { get; }
    public StaticObject WalnutTree { get; }
    public JobHarvest CutOak { get; }
    public JobHarvest CutPine { get; }
    public JobHarvest CutMaple { get; }
    public JobHarvest CutBirch { get; }
    public JobHarvest CutWalnut { get; }
}
