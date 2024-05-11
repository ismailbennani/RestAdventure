using RestAdventure.Core;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Spawners;

namespace ContentToolbox.Spawners;

public class RandomSpawner : Spawner
{
    readonly SpawnerLocationSelector _locationSelector;
    readonly EntitySpawner _entitySpawner;
    int _totalCount;
    readonly Dictionary<Location, int> _countPerLocation = new();
    long _globalRespawnDelay;
    readonly Dictionary<Location, long> _locationRespawnDelays = new();

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
    public (int Min, int Max)? RespawnDelay { get; set; }

    /// <summary>
    ///     If set, the max number of spawned entities per execution of the spawner. The spawner will stop after this number of entities even if <see cref="MaxCount" /> is not
    ///     reached yet. If more spawning is still required, it will resume spawning on the next tick.
    /// </summary>
    public int? MaxSpawnPerExecution { get; set; }

    /// <summary>
    ///     Fill the area with entities. The <see cref="MaxCount" /> and <see cref="MaxCountPerLocation" /> constraints will be enforced
    ///     but the <see cref="RespawnDelay" /> and <see cref="MaxCountPerLocation" /> will not.
    /// </summary>
    protected override IEnumerable<GameEntity> GetInitialEntitiesInternal(GameState state) => GetEntitiesToSpawnInternal(state, true, null);

    /// <summary>
    ///     Spawn entities while enforcing the <see cref="MaxCount" />, <see cref="MaxCountPerLocation" />, <see cref="RespawnDelay" /> and <see cref="MaxCountPerLocation" />
    ///     constraints
    /// </summary>
    protected override IEnumerable<GameEntity> GetEntitiesToSpawnInternal(GameState state) => GetEntitiesToSpawnInternal(state, false, MaxSpawnPerExecution);

    IEnumerable<GameEntity> GetEntitiesToSpawnInternal(GameState state, bool ignoreRespawnDelays, int? maxSpawn)
    {
        Random random = Random.Shared;

        TickRespawnDelays();

        if (!ignoreRespawnDelays && _globalRespawnDelay > 0)
        {
            yield break;
        }

        if (_totalCount >= MaxCount)
        {
            yield break;
        }

        HashSet<Location> suitableLocations = GetSuitableLocations(state, ignoreRespawnDelays).ToHashSet();

        if (suitableLocations.Count == 0)
        {
            yield break;
        }

        int totalSpawned = 0;
        while (suitableLocations.Count > 0 && (!maxSpawn.HasValue || totalSpawned < maxSpawn) && (!MaxCount.HasValue || _totalCount < MaxCount))
        {
            Location randomLocation = random.Choose(suitableLocations);

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

        if (RespawnDelay.HasValue && suitableLocations.Count == 0)
        {
            _globalRespawnDelay = random.Next(RespawnDelay.Value.Min, RespawnDelay.Value.Max + 1);
        }
    }

    protected override void OnEntityDeletedInternal(IGameEntity entity)
    {
        if (_countPerLocation.ContainsKey(entity.Location))
        {
            _countPerLocation[entity.Location] -= 1;
        }

        if (RespawnDelay.HasValue)
        {
            Random random = Random.Shared;

            if (_globalRespawnDelay <= 0)
            {
                _globalRespawnDelay = random.Next(RespawnDelay.Value.Min, RespawnDelay.Value.Max + 1);
            }

            if (_locationRespawnDelays.TryGetValue(entity.Location, out long value) && value <= 0)
            {
                _locationRespawnDelays[entity.Location] = random.Next(RespawnDelay.Value.Min, RespawnDelay.Value.Max);
            }
        }
    }

    IEnumerable<Location> GetSuitableLocations(GameState state, bool ignoreRespawnDelays)
    {
        IEnumerable<Location> locations = _locationSelector.GetLocations(state);

        if (!ignoreRespawnDelays)
        {
            locations = locations.Where(l => _locationRespawnDelays.GetValueOrDefault(l) <= 0);
        }

        if (MaxCountPerLocation.HasValue)
        {
            locations = locations.Where(l => _countPerLocation.GetValueOrDefault(l) < MaxCountPerLocation);
        }

        return locations;
    }

    void TickRespawnDelays()
    {
        if (_globalRespawnDelay > 0)
        {
            _globalRespawnDelay--;
        }

        foreach (Location location in _locationRespawnDelays.Keys)
        {
            if (_locationRespawnDelays[location] > 0)
            {
                _locationRespawnDelays[location]--;
            }
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
