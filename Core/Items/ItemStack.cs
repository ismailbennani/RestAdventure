﻿namespace RestAdventure.Core.Items;

public class ItemStack
{
    public ItemStack(Item item, int count)
    {
        Item = item;
        Count = count;
    }

    /// <summary>
    ///     The item representing this stack. All the items in the stack are identical to this one.
    /// </summary>
    public Item Item { get; }

    /// <summary>
    ///     The number of items in the stack. This value is always strictly positive.
    /// </summary>
    public int Count { get; internal set; }
}
