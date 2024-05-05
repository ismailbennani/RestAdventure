using RestAdventure.Kernel;

namespace RestAdventure.Core.Items;

public record ItemInstanceId(Guid Guid) : Id(Guid);

/// <summary>
///     Instance of an <see cref="Item" />.
/// </summary>
public class ItemInstance : IEquatable<ItemInstance>
{
    /// <summary>
    ///     The unique ID of the instance
    /// </summary>
    public ItemInstanceId Id { get; } = new(Guid.NewGuid());

    /// <summary>
    ///     The item that is instantiated
    /// </summary>
    public required Item Item { get; init; }

    public override string ToString() => $"{Id} ({Item})";

    public bool Equals(ItemInstance? other)
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
        return Equals((ItemInstance)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(ItemInstance? left, ItemInstance? right) => Equals(left, right);

    public static bool operator !=(ItemInstance? left, ItemInstance? right) => !Equals(left, right);
}
