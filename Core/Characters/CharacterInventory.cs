﻿using RestAdventure.Core.Items;

namespace RestAdventure.Core.Characters;

public class CharacterInventory : IInventory
{
    readonly Character _character;
    readonly Inventory _inventory = new();

    public CharacterInventory(Character character)
    {
        _character = character;
    }

    public IReadOnlyCollection<ItemStack> Stacks => _inventory.Stacks;
    public int Weight { get; private set; }

    public ItemStack Add(ItemInstance item, int count)
    {
        Weight += item.Item.Weight * count;
        return _inventory.Add(item, count);
    }

    public bool TryRemove(Item item, int count)
    {
        if (_inventory.TryRemove(item, count))
        {
            Weight -= item.Weight * count;
            return true;
        }

        return false;
    }

    public bool TryRemove(ItemInstance itemInstance, int count)
    {
        if (_inventory.TryRemove(itemInstance, count))
        {
            Weight -= itemInstance.Item.Weight * count;
            return true;
        }

        return false;
    }

    public int GetCountOf(Item item) => _inventory.GetCountOf(item);

    public ItemStack? Find(Item item) => _inventory.Find(item);

    public IEnumerable<ItemStack> FindAll(Item item) => _inventory.FindAll(item);

    public ItemStack? Find(ItemInstance itemInstance) => _inventory.Find(itemInstance);
}
