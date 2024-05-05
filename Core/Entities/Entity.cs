using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities;

public record EntityId(Guid Guid);

/// <inheritdoc cref="Entity{TId}" />
/// <remarks>
///     Common representation of all <see cref="Entity{TId}" /> instances
/// </remarks>
public abstract class Entity : IEquatable<Entity>
{
    public Entity(EntityId id, string name, Location location)
    {
        Id = id;
        Name = name;
        Location = location;
    }

    /// <summary>
    ///     The unique ID of the entity
    /// </summary>
    public EntityId Id { get; }

    /// <summary>
    ///     The name of the entity
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    ///     The location of the entity
    /// </summary>
    public Location Location { get; protected set; }

    public bool Equals(Entity? other)
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
        return Equals((Entity)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);

    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}

/// <summary>
///     An entity is something that exists in the world, at a specific location.
/// </summary>
public abstract class Entity<TId> : Entity where TId: EntityId
{
    public Entity(TId id, string name, Location location) : base(id, name, location)
    {
        Id = id;
    }

    /// <summary>
    ///     The unique ID of the entity
    /// </summary>
    public new TId Id { get; }
}
