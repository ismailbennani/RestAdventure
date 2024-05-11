using RestAdventure.Core;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Spawners;

namespace ContentToolbox.Spawners;

public class RandomSpawner : Spawner
{
    long _lastTickWhenMaxCountWasReached;
    readonly SpawnerLocationSelector _locationSelector;
    readonly EntitySpawner _entitySpawner;
    int _totalCount;
    readonly Dictionary<Location, int> _countPerLocation;

    public RandomSpawner(SpawnerLocationSelector locationSelector, EntitySpawner entitySpawner)
    {
        _locationSelector = locationSelector;
        _entitySpawner = entitySpawner;
        _countPerLocation = new Dictionary<Location, int>();
    }

    /// <summary>
    ///     The max number of entities in the area. The spawner will not spawn an entity if this count is reached.
    /// </summary>
    public int? MaxCount { get; set; }

    /// <summary>
    ///     If set, the max number of entities per location. The spawner will ignore the locations of the area where this max count has been reached
    /// </summary>
    public int? MaxCountPerLocation { get; set; }

    /// <summary>
    ///     If set, the respawn delay. This delay is applied between the tick where spawning is required (i.e. the tick where MaxCountInArea is no longer fulfilled) and the tick where
    ///     the spawning actually occurs.
    /// </summary>
    public int? RespawnDelay { get; set; }

    /// <summary>
    ///     If set, the max number of spawned entities per execution of the spawner. The spawner will stop after this number of entities even if <see cref="MaxCount" /> is not
    ///     reached yet. If more spawning is still required, it will resume spawning on the next tick.
    /// </summary>
    public int? MaxSpawnPerExecution { get; set; }

    /// <summary>
    ///     Fill the area with entities. The <see cref="MaxCount" /> and <see cref="MaxCountPerLocation" /> constraints will be enforced
    ///     but the <see cref="RespawnDelay" /> and <see cref="MaxCountPerLocation" /> will not.
    /// </summary>
    protected override IEnumerable<GameEntity> GetInitialEntitiesInternal(GameState state)
    {
        foreach (Location location in _locationSelector.GetLocations(state))
        {
            _countPerLocation[location] = 0;
        }

        return GetEntitiesToSpawnInternal(state, null, null);
    }

    /// <summary>
    ///     Spawn entities while enforcing the <see cref="MaxCount" />, <see cref="MaxCountPerLocation" />, <see cref="RespawnDelay" /> and <see cref="MaxCountPerLocation" />
    ///     constraints
    /// </summary>
    protected override IEnumerable<GameEntity> GetEntitiesToSpawnInternal(GameState state) => GetEntitiesToSpawnInternal(state, RespawnDelay, MaxSpawnPerExecution);

    IEnumerable<GameEntity> GetEntitiesToSpawnInternal(GameState state, int? respawnDelay, int? maxSpawn)
    {
        if (respawnDelay.HasValue && state.Tick - _lastTickWhenMaxCountWasReached < respawnDelay)
        {
            yield break;
        }

        if (_totalCount >= MaxCount)
        {
            _lastTickWhenMaxCountWasReached = state.Tick;
            yield break;
        }

        HashSet<Location> suitableLocations = _locationSelector.GetLocations(state).ToHashSet();

        if (MaxCountPerLocation.HasValue)
        {
            suitableLocations.RemoveWhere(l => _countPerLocation.GetValueOrDefault(l) >= MaxCountPerLocation);
        }

        if (suitableLocations.Count == 0)
        {
            yield break;
        }

        int totalSpawned = 0;
        while (suitableLocations.Count > 0 && (!maxSpawn.HasValue || totalSpawned < maxSpawn) && (!MaxCount.HasValue || _totalCount < MaxCount))
        {
            Location randomLocation = Random.Shared.Choose(suitableLocations);

            if (!_countPerLocation.TryAdd(randomLocation, 1))
            {
                _countPerLocation[randomLocation] += 1;
            }
            totalSpawned++;
            _totalCount++;


            foreach (GameEntity entity in _entitySpawner.Spawn(randomLocation))
            {
                yield return entity;
            }

            if (MaxCountPerLocation.HasValue && _countPerLocation[randomLocation] >= MaxCountPerLocation)
            {
                suitableLocations.Remove(randomLocation);
            }
        }

        if (suitableLocations.Count == 0)
        {
            _lastTickWhenMaxCountWasReached = state.Tick;
        }
    }
}

public abstract class SpawnerLocationSelector
{
    public abstract IEnumerable<Location> GetLocations(GameState state);
}

public abstract class EntitySpawner
{
    public abstract IEnumerable<GameEntity> Spawn(Location location);
}
