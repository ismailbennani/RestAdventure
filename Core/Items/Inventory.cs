namespace RestAdventure.Core.Items;

/// <summary>
///     A collection of items
/// </summary>
public class Inventory : IReadOnlyInventory
{
    readonly List<ItemInstanceStack> _stacks = [];

    public IReadOnlyCollection<ItemInstanceStack> Stacks => _stacks;
    public int Weight { get; private set; }

    public event EventHandler<InventoryItemStackChangedEvent>? Changed;

    /// <summary>
    ///     Add an item to the inventory. The item will be stacked with existing items if it is stackable.
    /// </summary>
    public ItemInstanceStack Add(ItemInstance itemInstance, int count)
    {
        ItemInstanceStack? stack = Find(itemInstance.Item);

        int oldCount;
        if (stack == null)
        {
            oldCount = 0;
            stack = new ItemInstanceStack(itemInstance, count);
            _stacks.Add(stack);
        }
        else
        {
            oldCount = stack.Count;
            stack.Count += count;
        }

        Weight += itemInstance.Item.Weight * count;

        Changed?.Invoke(this, new InventoryItemStackChangedEvent { ItemInstance = itemInstance, OldCount = oldCount, NewCount = stack.Count });

        return stack;
    }

    /// <summary>
    ///     Remove the given count of items if there are enough in the inventory.
    ///     This can remove multiple stacks of items if necessary, until the requested count has been reached.
    /// </summary>
    /// <seealso cref="TryRemove(RestAdventure.Core.Items.ItemInstance,int)" />
    public bool TryRemove(Item item, int count)
    {
        int totalCount = GetCountOf(item);
        if (totalCount < count)
        {
            return false;
        }

        int removed = 0;
        while (removed < count)
        {
            ItemInstanceStack? nextStack = Find(item);
            if (nextStack == null)
            {
                throw new InvalidOperationException("Internal error: there should be enough items to remove");
            }

            removed += nextStack.Count;

            if (!TryRemove(nextStack.ItemInstance, nextStack.Count))
            {
                throw new InvalidOperationException("Internal error: there should be enough items to remove");
            }
        }

        return true;
    }

    /// <summary>
    ///     Remove the given count of items if there are enough in the inventory.
    ///     This can only remove one stack of items: the one represented by the given item instance.
    /// </summary>
    /// <seealso cref="TryRemove(RestAdventure.Core.Items.Item,int)" />
    public bool TryRemove(ItemInstance itemInstance, int count)
    {
        ItemInstanceStack? stack = Find(itemInstance);
        if (stack == null || stack.Count < count)
        {
            return false;
        }

        int oldCount = stack.Count;
        stack.Count -= count;
        if (stack.Count <= 0)
        {
            _stacks.Remove(stack);
        }

        Weight -= itemInstance.Item.Weight * count;

        Changed?.Invoke(this, new InventoryItemStackChangedEvent { ItemInstance = itemInstance, OldCount = oldCount, NewCount = stack.Count });

        return true;
    }

    /// <inheritdoc />
    public int GetCountOf(Item item) => Find(item)?.Count ?? 0;

    /// <inheritdoc />
    public ItemInstanceStack? Find(Item item) => _stacks.FirstOrDefault(e => e.ItemInstance.Item == item);

    /// <inheritdoc />
    public IEnumerable<ItemInstanceStack> FindAll(Item item) => _stacks.Where(e => e.ItemInstance.Item == item);

    /// <inheritdoc />
    public ItemInstanceStack? Find(ItemInstance itemInstance) => _stacks.FirstOrDefault(e => e.ItemInstance == itemInstance);
}

public class InventoryItemStackChangedEvent
{
    public required ItemInstance ItemInstance { get; init; }
    public int OldCount { get; init; }
    public int NewCount { get; init; }
}

public static class InventoryExtensions
{
    public static ItemInstanceStack Add(this Inventory inventory, ItemInstanceStack stack) => inventory.Add(stack.ItemInstance, stack.Count);

    public static IReadOnlyCollection<ItemInstanceStack> Add(this Inventory inventory, IEnumerable<ItemInstanceStack> stacks) =>
        stacks.Select(inventory.Add).DistinctBy(s => s.ItemInstance).ToArray();

    public static ItemInstanceStack Add(this Inventory inventory, ItemStack stack) => inventory.Add(new ItemInstance(stack.Item), stack.Count);

    public static IReadOnlyCollection<ItemInstanceStack> Add(this Inventory inventory, IEnumerable<ItemStack> stacks) =>
        stacks.Select(inventory.Add).DistinctBy(s => s.ItemInstance).ToArray();
}
