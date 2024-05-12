using RestAdventure.Core.Items;

namespace SandboxGame.Items;

public class ResourceCategories
{
    public ResourceCategories()
    {
        CommonResource = new ItemCategory { Name = "Common resource" };
        OreCategory = new ItemCategory { Name = "Ore" };
        PlantCategory = new ItemCategory { Name = "Plant" };
        WoodCategory = new ItemCategory { Name = "Wood" };
    }

    public ItemCategory CommonResource { get; }
    public ItemCategory OreCategory { get; }
    public ItemCategory PlantCategory { get; }
    public ItemCategory WoodCategory { get; }
}
