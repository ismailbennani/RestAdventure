using RestAdventure.Kernel;

namespace RestAdventure.Core.Items;

public record ItemId(Guid Guid) : Id(Guid);

/// <summary>
///     An item.
///     There should be only one instance of this class per item. It stores all the meta data about an item: its name, description, etc...
///     The materialization of an item in the game world is <see cref="ItemInstance" />.
/// </summary>
public class Item : IEquatable<Item>
{
    /// <summary>
    ///     The unique ID of the item
    /// </summary>
    public ItemId Id { get; } = new(Guid.NewGuid());

    /// <summary>
    ///     The name of the item
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The description of the item
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     The weight of the item
    /// </summary>
    public required int Weight { get; init; }

    public override string ToString() => $"{Name} ({Id})";

    public bool Equals(Item? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj.GetType() != GetType())
        {
            return false;
        }
        return Equals((Item)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Item? left, Item? right) => Equals(left, right);

    public static bool operator !=(Item? left, Item? right) => !Equals(left, right);
}
