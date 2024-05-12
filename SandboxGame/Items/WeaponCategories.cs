using RestAdventure.Core.Items;

namespace SandboxGame.Items;

public class WeaponCategories
{
    public WeaponCategories()
    {
        Axe = new ItemCategory { Name = "Axe" };
        Pickaxe = new ItemCategory { Name = "Pickaxe" };
    }

    public ItemCategory Axe { get; }
    public ItemCategory Pickaxe { get; }
}
