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

    public RandomSpawner(SpawnerLocationSelector locationSelector, EntitySpawner entitySpawner)
    {
        _locationSelector = locationSelector;
        _entitySpawner = entitySpawner;
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
    public override IEnumerable<GameEntity> GetInitialEntities(GameState state) => GetEntitiesToSpawnInternal(state, null, null);

    /// <summary>
    ///     Spawn entities while enforcing the <see cref="MaxCount" />, <see cref="MaxCountPerLocation" />, <see cref="RespawnDelay" /> and <see cref="MaxCountPerLocation" />
    ///     constraints
    /// </summary>
    public override IEnumerable<GameEntity> GetEntitiesToSpawn(GameState state) => GetEntitiesToSpawnInternal(state, RespawnDelay, MaxSpawnPerExecution);

    IEnumerable<GameEntity> GetEntitiesToSpawnInternal(GameState state, int? respawnDelay, int? maxSpawn)
    {
        if (respawnDelay.HasValue && state.Tick - _lastTickWhenMaxCountWasReached < respawnDelay)
        {
            yield break;
        }

        IEnumerable<Location> locations = _locationSelector.GetLocations(state);
        Dictionary<Location, int> suitableLocationsWithCounts = locations.ToDictionary(l => l, l => _entitySpawner.Count(state.Entities.AtLocation<GameEntity>(l)));

        int currentCount = suitableLocationsWithCounts.Values.Sum();
        if (currentCount >= MaxCount)
        {
            _lastTickWhenMaxCountWasReached = state.Tick;
            yield break;
        }

        if (MaxCountPerLocation.HasValue)
        {
            suitableLocationsWithCounts = new Dictionary<Location, int>(suitableLocationsWithCounts.Where(kv => kv.Value < MaxCountPerLocation));
        }

        Dictionary<Location, int> spawned = new();
        int totalSpawned = 0;
        while (suitableLocationsWithCounts.Count != 0 && (!maxSpawn.HasValue || totalSpawned < maxSpawn) && (!MaxCount.HasValue || currentCount + totalSpawned < MaxCount))
        {
            KeyValuePair<Location, int> randomLocationWithCount = Random.Shared.Choose(suitableLocationsWithCounts);

            if (!spawned.TryAdd(randomLocationWithCount.Key, 1))
            {
                spawned[randomLocationWithCount.Key] += 1;
            }
            totalSpawned++;

            foreach (GameEntity entity in _entitySpawner.Spawn(randomLocationWithCount.Key))
            {
                yield return entity;
            }

            if (MaxCountPerLocation.HasValue && randomLocationWithCount.Value + spawned[randomLocationWithCount.Key] >= MaxCountPerLocation)
            {
                suitableLocationsWithCounts.Remove(randomLocationWithCount.Key);
            }
        }

        if (suitableLocationsWithCounts.Count == 0)
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
    public abstract int Count(IEnumerable<GameEntity> entities);
    public abstract IEnumerable<GameEntity> Spawn(Location location);
}
