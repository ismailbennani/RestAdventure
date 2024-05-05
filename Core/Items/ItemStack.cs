namespace RestAdventure.Core.Items;

public class ItemStack
{
    internal ItemStack(ItemInstance itemInstance, int count)
    {
        ItemInstance = itemInstance;
        Count = count;
    }

    public ItemInstance ItemInstance { get; }
    public int Count { get; internal set; }
}
