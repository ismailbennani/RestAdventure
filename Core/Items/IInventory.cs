namespace RestAdventure.Core.Items;

/// <summary>
///     A collection of items
/// </summary>
public interface IInventory : IReadOnlyInventory
{
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
}
