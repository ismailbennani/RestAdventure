namespace RestAdventure.Core.Items;

/// <inheritdoc />
public class Inventory : IInventory
{
    readonly List<ItemStack> _stacks = [];

    internal Inventory() { }

    public IReadOnlyCollection<ItemStack> Stacks => _stacks;

    public event EventHandler<InventoryItemStackChangedEvent>? Changed;

    public ItemStack Add(ItemInstance itemInstance, int count)
    {
        ItemStack? stack = Find(itemInstance.Item);

        int oldCount;
        if (stack == null)
        {
            oldCount = 0;
            stack = new ItemStack(itemInstance, count);
            _stacks.Add(stack);
        }
        else
        {
            oldCount = stack.Count;
            stack.Count += count;
        }

        Changed?.Invoke(this, new InventoryItemStackChangedEvent { ItemInstance = itemInstance, OldCount = oldCount, NewCount = stack.Count });

        return stack;
    }

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
            ItemStack? nextStack = Find(item);
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

    public bool TryRemove(ItemInstance itemInstance, int count)
    {
        ItemStack? stack = Find(itemInstance);
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

        Changed?.Invoke(this, new InventoryItemStackChangedEvent { ItemInstance = itemInstance, OldCount = oldCount, NewCount = stack.Count });

        return true;
    }

    public int GetCountOf(Item item) => Find(item)?.Count ?? 0;

    public ItemStack? Find(Item item) => _stacks.FirstOrDefault(e => e.ItemInstance.Item == item);
    public IEnumerable<ItemStack> FindAll(Item item) => _stacks.Where(e => e.ItemInstance.Item == item);
    public ItemStack? Find(ItemInstance itemInstance) => _stacks.FirstOrDefault(e => e.ItemInstance == itemInstance);
}

public class InventoryItemStackChangedEvent
{
    public required ItemInstance ItemInstance { get; init; }
    public int OldCount { get; init; }
    public int NewCount { get; init; }
}
