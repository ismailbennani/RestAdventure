using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities;

public record GameEntityId(Guid Guid);

/// <inheritdoc cref="GameEntity{TId}" />
/// <remarks>
///     Common representation of all <see cref="GameEntity{TId}" /> instances
/// </remarks>
public abstract class GameEntity : IEquatable<GameEntity>, IGameEntity, IDisposable
{
    public GameEntity(GameEntityId id, string name, Location location)
    {
        Id = id;
        Name = name;
        Location = location;
    }

    /// <inheritdoc />
    public GameEntityId Id { get; }

    /// <inheritdoc />
    public string Name { get; protected set; }

    /// <inheritdoc />
    public Location Location { get; private set; }

    /// <inheritdoc />
    public event EventHandler<EntityMovedEvent>? Moved;

    public void MoveTo(Location location)
    {
        if (Location == location)
        {
            return;
        }

        Location oldLocation = Location;
        Location = location;

        Moved?.Invoke(this, new EntityMovedEvent { OldLocation = oldLocation, NewLocation = Location });
    }

    public async Task KillAsync(GameState state) => await state.Entities.DestroyAsync(this);

    public override string ToString() => Name;

    public virtual void Dispose()
    {
        Moved = null;
        GC.SuppressFinalize(this);
    }

    public bool Equals(GameEntity? other)
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
        return Equals((GameEntity)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(GameEntity? left, GameEntity? right) => Equals(left, right);

    public static bool operator !=(GameEntity? left, GameEntity? right) => !Equals(left, right);
}

public class EntityMovedEvent
{
    public required Location OldLocation { get; init; }
    public required Location NewLocation { get; init; }
}

/// <summary>
///     An entity is something that exists in the world, at a specific location.
/// </summary>
public abstract class GameEntity<TId> : GameEntity where TId: GameEntityId
{
    public GameEntity(TId id, string name, Location location) : base(id, name, location)
    {
        Id = id;
    }

    /// <summary>
    ///     The unique ID of the entity
    /// </summary>
    public new TId Id { get; }
}
