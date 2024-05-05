namespace RestAdventure.Core.Items;

/// <summary>
///     A read-only collection of items
/// </summary>
public interface IReadOnlyInventory
{
    /// <summary>
    ///     The entries in the inventory
    /// </summary>
    IReadOnlyCollection<ItemStack> Stacks { get; }

    /// <summary>
    ///     The total weight of all the items in the inventory
    /// </summary>
    int Weight { get; }

    /// <summary>
    ///     Event fired each time a stack of the inventory changes
    /// </summary>
    event EventHandler<InventoryItemStackChangedEvent> Changed;

    /// <summary>
    ///     Get the total count of the given item. If there are multiple stacks of items, this will return the sum of their sizes.
    /// </summary>
    int GetCountOf(Item item);

    /// <summary>
    ///     Find the first stack of the given item. There might be more stacks.
    /// </summary>
    ItemStack? Find(Item item);

    /// <summary>
    ///     Find all the stacks of the given item.
    /// </summary>
    IEnumerable<ItemStack> FindAll(Item item);

    /// <summary>
    ///     Find the stack represented by the given item instance.
    /// </summary>
    ItemStack? Find(ItemInstance itemInstance);
}
