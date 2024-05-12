using RestAdventure.Core.Items;

namespace SandboxGame.Items;

public class GenericItemCategories
{
    public GenericItemCategories()
    {
        CommonResource = new ItemCategory
        {
            Name = "Common resource"
        };
    }

    public ItemCategory CommonResource { get; }
}
