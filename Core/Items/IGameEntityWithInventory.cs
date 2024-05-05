using RestAdventure.Core.Entities;

namespace RestAdventure.Core.Items;

public interface IGameEntityWithInventory : IGameEntity
{
    Inventory Inventory { get; }
}
