namespace RestAdventure.Core.Items;

public class ItemStack
{
    internal ItemStack(ItemInstance itemInstance, int count)
    {
        ItemInstance = itemInstance;
        Count = count;
    }

    /// <summary>
    ///     The item representing this stack. All the items in the stack are identical to this one.
    /// </summary>
    public ItemInstance ItemInstance { get; }

    /// <summary>
    ///     The number of items in the stack. This value is always strictly positive.
    /// </summary>
    public int Count { get; internal set; }
}
