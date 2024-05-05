namespace RestAdventure.Core.Resources;

public record ResourceId(Guid Guid);

/// <inheritdoc cref="GameResource{TId}" />
/// <remarks>
///     Common representation of all <see cref="GameResource{TId}" /> instances
/// </remarks>
public abstract class GameResource : IEquatable<GameResource>
{
    protected GameResource(ResourceId id)
    {
        Id = id;
    }

    /// <summary>
    ///     The unique ID of the resource
    /// </summary>
    public ResourceId Id { get; }

    public override string ToString() => $"{Id}";

    public bool Equals(GameResource? other)
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
        return Equals((GameResource)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(GameResource? left, GameResource? right) => Equals(left, right);

    public static bool operator !=(GameResource? left, GameResource? right) => !Equals(left, right);
}

/// <summary>
///     A piece of data in the game.
///     This common representation is used to encapsulate common behaviors for all the knowledge of the game.
///     For example most resources should be hidden from the players until at least one of their characters discover them.
/// </summary>
public abstract class GameResource<TId> : GameResource where TId: ResourceId
{
    protected GameResource(TId id) : base(id)
    {
        Id = id;
    }

    /// <summary>
    ///     The unique ID of the resource
    /// </summary>
    public new TId Id { get; }
}
