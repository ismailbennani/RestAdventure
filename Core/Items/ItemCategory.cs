using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Items;

public record ItemCategoryId(Guid Guid) : ResourceId(Guid);

public class ItemCategory : GameResource<ItemCategoryId>
{
    public ItemCategory() : base(new ItemCategoryId(Guid.NewGuid()))
    {
    }

    public required string Name { get; init; }
}
