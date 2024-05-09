using RestAdventure.Core.Entities;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Spawners;

public record SpawnerId(Guid Guid) : ResourceId(Guid);

/// <summary>
///     Resource that spawns entities during gameplay
/// </summary>
public abstract class Spawner : GameResource<SpawnerId>, IEquatable<Spawner>, IGameEntitySource
{
    protected Spawner() : base(new SpawnerId(Guid.NewGuid()))
    {
    }

    public virtual IEnumerable<GameEntity> GetInitialEntities(GameState state) => Enumerable.Empty<GameEntity>();
    public abstract IEnumerable<GameEntity> GetEntitiesToSpawn(GameState state);

    public bool Equals(Spawner? other)
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
        return Equals((Spawner)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Spawner? left, Spawner? right) => Equals(left, right);

    public static bool operator !=(Spawner? left, Spawner? right) => !Equals(left, right);
}
