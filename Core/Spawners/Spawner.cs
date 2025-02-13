﻿using RestAdventure.Core.Entities;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Spawners;

public record SpawnerId(Guid Guid) : ResourceId(Guid);

/// <summary>
///     Resource that spawns entities during gameplay
/// </summary>
public abstract class Spawner : GameResource<SpawnerId>, IEquatable<Spawner>, IGameEntitySource
{
    readonly HashSet<IGameEntity> _entities = new();

    protected Spawner() : base(new SpawnerId(Guid.NewGuid()))
    {
    }


    public IEnumerable<GameEntity> GetInitialEntities(Game state)
    {
        IEnumerable<GameEntity> entities = GetInitialEntitiesInternal(state);
        foreach (GameEntity entity in entities)
        {
            entity.Source = this;
            Remember(entity);
            yield return entity;
        }
    }

    public IEnumerable<GameEntity> GetEntitiesToSpawn(Game state)
    {
        IEnumerable<GameEntity> entities = GetEntitiesToSpawnInternal(state);
        foreach (GameEntity entity in entities)
        {
            entity.Source = this;
            Remember(entity);
            yield return entity;
        }
    }

    public void OnEntityDeleted(IGameEntity entity)
    {
        Forget(entity);
        OnEntityDeletedInternal(entity);
    }

    void Remember(IGameEntity entity) => _entities.Add(entity);
    void Forget(IGameEntity entity) => _entities.Remove(entity);

    protected virtual IEnumerable<GameEntity> GetInitialEntitiesInternal(Game state) => Enumerable.Empty<GameEntity>();
    protected abstract IEnumerable<GameEntity> GetEntitiesToSpawnInternal(Game state);
    protected virtual void OnEntityDeletedInternal(IGameEntity entity) { }

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
