using RestAdventure.Core.Entities;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Spawners;

/// <summary>
///     Spawn entities randomly in an area
/// </summary>
public abstract class RandomAreaSpawner<TEntity> : Spawner where TEntity: GameEntity
{
    public RandomAreaSpawner(MapArea area, int maxCount)
    {
        Area = area;
        MaxCount = maxCount;
    }

    public MapArea Area { get; }
    public int MaxCount { get; }
    public int? MaxCountPerLocation { get; set; }
    public int? RespawnDelay { get; set; }
    public int? MaxSpawnPerExecution { get; set; }

    public long LastTickWhenMaxCountWasReached { get; private set; }

    public override IEnumerable<GameEntity> GetInitialEntities(GameState state) => GetEntitiesToSpawnInternal(state, null, null);
    public override IEnumerable<GameEntity> GetEntitiesToSpawn(GameState state) => GetEntitiesToSpawnInternal(state, RespawnDelay, MaxSpawnPerExecution);

    public abstract TEntity Spawn(Location location);
    public abstract int Count(IEnumerable<TEntity> entities);

    IEnumerable<GameEntity> GetEntitiesToSpawnInternal(GameState state, int? respawnDelay, int? maxSpawn)
    {
        if (respawnDelay.HasValue && state.Tick - LastTickWhenMaxCountWasReached < respawnDelay)
        {
            yield break;
        }

        Dictionary<Location, int> suitableLocationsWithCounts = state.Content.Maps.Locations.InArea(Area).ToDictionary(l => l, l => Count(state.Entities.AtLocation<TEntity>(l)));

        int currentCount = suitableLocationsWithCounts.Values.Sum();
        if (currentCount >= MaxCount)
        {
            LastTickWhenMaxCountWasReached = state.Tick;
            yield break;
        }

        if (MaxCountPerLocation.HasValue)
        {
            suitableLocationsWithCounts = new Dictionary<Location, int>(suitableLocationsWithCounts.Where(kv => kv.Value < MaxCountPerLocation));
        }

        Dictionary<Location, int> spawned = new();
        int totalSpawned = 0;
        while (suitableLocationsWithCounts.Count != 0 && (!maxSpawn.HasValue || totalSpawned < maxSpawn) && currentCount + totalSpawned < MaxCount)
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
            LastTickWhenMaxCountWasReached = state.Tick;
        }
    }
}
