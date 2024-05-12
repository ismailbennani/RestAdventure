using RestAdventure.Core.Entities.Notifications;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Entities;

public record GameEntityId(Guid Guid);

/// <inheritdoc cref="GameEntity{TId}" />
/// <remarks>
///     Common representation of all <see cref="GameEntity{TId}" /> instances
/// </remarks>
public abstract class GameEntity : IEquatable<GameEntity>, IGameEntity
{
    public GameEntity(GameEntityId id, string name, Location location) : this(null, id, name, location) { }

    public GameEntity(Team? team, GameEntityId id, string name, Location location)
    {
        Team = team;
        Id = id;
        Name = name;
        Location = location;
    }

    /// <inheritdoc />
    public GameEntityId Id { get; }

    /// <inheritdoc />
    public IGameEntitySource? Source { get; set; }

    /// <inheritdoc />
    public Team? Team { get; }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public Location Location { get; set; }

    /// <inheritdoc />
    public bool Busy { get; set; }

    public async Task TeleportAsync(Game state, Location location)
    {
        Location oldLocation = Location;
        Location = location;

        await state.Publisher.Publish(new GameEntityTeleportedToLocation { Entity = this, OldLocation = oldLocation, NewLocation = Location });
    }

    public virtual async Task KillAsync(Game state) => await state.Entities.DestroyAsync(this);

    public override string ToString() => Name;

    public virtual void Dispose() => GC.SuppressFinalize(this);

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
    public GameEntity(TId id, string name, Location location) : this(id, null, name, location)
    {
    }

    protected GameEntity(TId id, Team? team, string name, Location location) : base(team, id, name, location)
    {
        Id = id;
    }

    /// <summary>
    ///     The unique ID of the entity
    /// </summary>
    public new TId Id { get; }
}
