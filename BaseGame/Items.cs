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
}
