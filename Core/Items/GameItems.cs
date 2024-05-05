namespace RestAdventure.Core.Items;

public class GameItems
{
    readonly Dictionary<ItemId, Item> _items = [];

    public IEnumerable<Item> All => _items.Values;

    public void Register(Item item) => _items[item.Id] = item;
    public Item? Get(ItemId itemId) => _items.GetValueOrDefault(itemId);
}

public static class GameItemsExtensions
{
    public static Item RequireItem(this GameItems items, ItemId itemId) => items.Get(itemId) ?? throw new InvalidOperationException($"Could not find item {itemId}");
}
