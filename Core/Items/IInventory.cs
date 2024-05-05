namespace RestAdventure.Core.Items;

/// <summary>
///     A collection of items
/// </summary>
public interface IInventory
{
    /// <summary>
    ///     The entries in the inventory
    /// </summary>
    IReadOnlyCollection<ItemStack> Stacks { get; }

    /// <summary>
    ///     Add an item to the inventory. The item will be stacked with existing items if it is stackable.
    /// </summary>
    ItemStack Add(ItemInstance item, int count);

    /// <summary>
    ///     Remove the given count of items if there are enough in the inventory.
    ///     This can remove multiple stacks of items if necessary, until the requested count has been reached.
    /// </summary>
    /// <seealso cref="TryRemove(RestAdventure.Core.Items.ItemInstance,int)" />
    bool TryRemove(Item item, int count);

    /// <summary>
    ///     Remove the given count of items if there are enough in the inventory.
    ///     This can only remove one stack of items: the one represented by the given item instance.
    /// </summary>
    /// <seealso cref="TryRemove(RestAdventure.Core.Items.Item,int)" />
    bool TryRemove(ItemInstance itemInstance, int count);

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
