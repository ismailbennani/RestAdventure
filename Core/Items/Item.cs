using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Items;

public record ItemId(Guid Guid) : ResourceId(Guid);

/// <summary>
///     An item.
///     There should be only one instance of this class per item. It stores all the meta data about an item: its name, description, etc...
///     The materialization of an item in the game world is <see cref="ItemInstance" />.
/// </summary>
public class Item : GameResource<ItemId>
{
    public Item() : base(new ItemId(Guid.NewGuid())) { }

    /// <summary>
    ///     The name of the item
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The description of the item
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     The category of the item
    /// </summary>
    public required ItemCategory Category { get; init; }

    /// <summary>
    ///     The weight of the item
    /// </summary>
    public required int Weight { get; init; }

    public override string ToString() => $"Item {Name} ({Id})";
}
