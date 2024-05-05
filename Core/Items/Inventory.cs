namespace RestAdventure.Core.Items;

/// <inheritdoc />
public class Inventory : IInventory
{
    readonly List<ItemStack> _stacks = [];

    internal Inventory() { }

    public IReadOnlyCollection<ItemStack> Stacks => _stacks;

    public ItemStack Add(ItemInstance item, int count)
    {
        ItemStack? stack = Find(item.Item);

        if (stack == null)
        {
            stack = new ItemStack(item, count);
            _stacks.Add(stack);
        }
        else
        {
            stack.Count += count;
        }

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

        stack.Count -= count;
        if (stack.Count <= 0)
        {
            _stacks.Remove(stack);
        }

        return true;
    }

    public int GetCountOf(Item item) => Find(item)?.Count ?? 0;

    public ItemStack? Find(Item item) => _stacks.FirstOrDefault(e => e.ItemInstance.Item == item);
    public IEnumerable<ItemStack> FindAll(Item item) => _stacks.Where(e => e.ItemInstance.Item == item);
    public ItemStack? Find(ItemInstance itemInstance) => _stacks.FirstOrDefault(e => e.ItemInstance == itemInstance);
}
