namespace RestAdventure.Core.Items;

public class GameItems
{
    readonly Dictionary<ItemId, Item> _items = [];

    public Item CreateItem(string name, string description, int weight)
    {
        Item item = new(name, description, weight);
        _items[item.Id] = item;

        return item;
    }

    public Item? GetItem(ItemId itemId) => _items.GetValueOrDefault(itemId);
}

public static class GameItemsExtensions
{
    public static Item RequireItem(this GameItems items, ItemId itemId) => items.GetItem(itemId) ?? throw new InvalidOperationException($"Could not find item {itemId}");
}
