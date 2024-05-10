using RestAdventure.Core;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Spawners;

namespace ContentToolbox.Spawners;

/// <summary>
///     Spawn entities randomly in an area
/// </summary>
public abstract class RandomAreaSpawner<TEntity> : Spawner where TEntity: GameEntity
{
    long _lastTickWhenMaxCountWasReached;

    public RandomAreaSpawner(MapArea area, int maxCountInArea)
    {
        Area = area;
        MaxCountInArea = maxCountInArea;
    }

    /// <summary>
    ///     The area of the spawner
    /// </summary>
    public MapArea Area { get; }

    /// <summary>
    ///     The max number of entities in the area. The spawner will not spawn an entity if this count is reached.
    /// </summary>
    public int MaxCountInArea { get; }

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
    ///     If set, the max number of spawned entities per execution of the spawner. The spawner will stop after this number of entities even if <see cref="MaxCountInArea" /> is not
    ///     reached yet. If more spawning is still required, it will resume spawning on the next tick.
    /// </summary>
    public int? MaxSpawnPerExecution { get; set; }

    /// <summary>
    ///     Fill the area with entities. The <see cref="MaxCountInArea" /> and <see cref="MaxCountPerLocation" /> constraints will be enforced
    ///     but the <see cref="RespawnDelay" /> and <see cref="MaxCountPerLocation" /> will not.
    /// </summary>
    public override IEnumerable<GameEntity> GetInitialEntities(GameState state) => GetEntitiesToSpawnInternal(state, null, null);

    /// <summary>
    ///     Spawn entities while enforcing the <see cref="MaxCountInArea" />, <see cref="MaxCountPerLocation" />, <see cref="RespawnDelay" /> and <see cref="MaxCountPerLocation" />
    ///     constraints
    /// </summary>
    public override IEnumerable<GameEntity> GetEntitiesToSpawn(GameState state) => GetEntitiesToSpawnInternal(state, RespawnDelay, MaxSpawnPerExecution);

    /// <summary>
    ///     Spawn an element at the given location
    /// </summary>
    public abstract TEntity Spawn(Location location);

    /// <summary>
    ///     Count the number of entities that should be used to enforce the <see cref="MaxCountInArea" /> and <see cref="MaxCountPerLocation" /> constraints.
    /// </summary>
    public abstract int Count(IEnumerable<TEntity> entities);

    IEnumerable<GameEntity> GetEntitiesToSpawnInternal(GameState state, int? respawnDelay, int? maxSpawn)
    {
        if (respawnDelay.HasValue && state.Tick - _lastTickWhenMaxCountWasReached < respawnDelay)
        {
            yield break;
        }

        Dictionary<Location, int> suitableLocationsWithCounts = state.Content.Maps.Locations.InArea(Area).ToDictionary(l => l, l => Count(state.Entities.AtLocation<TEntity>(l)));

        int currentCount = suitableLocationsWithCounts.Values.Sum();
        if (currentCount >= MaxCountInArea)
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
        while (suitableLocationsWithCounts.Count != 0 && (!maxSpawn.HasValue || totalSpawned < maxSpawn) && currentCount + totalSpawned < MaxCountInArea)
        {
            KeyValuePair<Location, int> randomLocationWithCount = Random.Shared.Choose(suitableLocationsWithCounts);

            if (!spawned.TryAdd(randomLocationWithCount.Key, 1))
            {
                spawned[randomLocationWithCount.Key] += 1;
            }
            totalSpawned++;

            yield return Spawn(randomLocationWithCount.Key);

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
