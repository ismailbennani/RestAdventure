using System.Reflection;
using RestAdventure.Core.Items;

public class Items
{
    public Items()
    {
        Apple = new Item { Name = "Apple", Description = "A delicious apple.", Weight = 1 };
        Pear = new Item { Name = "Pear", Description = "A very delicious pear.", Weight = 1 };
    }

    public Item Apple { get; }
    public Item Pear { get; }

    public IEnumerable<Item> All =>
        typeof(Items).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.PropertyType == typeof(Item)).Select(p => p.GetValue(this)).OfType<Item>();
}
