using ContentToolbox.Noise;
using Microsoft.Extensions.Logging;
using RestAdventure.Core;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Spawners;

namespace ContentToolbox.Spawners;

public class RandomSpawner : Spawner
{
    readonly Noise2D? _noise;
    readonly SpawnerLocationSelector? _locationSelector;
    readonly EntitySpawner _entitySpawner;
    readonly ILogger<RandomSpawner> _logger;
    int _totalCount;
    readonly Dictionary<Location, int> _countPerLocation = new();
    long _globalRespawnDelay;
    readonly Dictionary<Location, long> _locationRespawnDelays = new();

    public RandomSpawner(EntitySpawner entitySpawner, ILogger<RandomSpawner> logger) : this(null, null, entitySpawner, logger)
    {
    }

    public RandomSpawner(Noise2D noise, EntitySpawner entitySpawner, ILogger<RandomSpawner> logger) : this(noise, null, entitySpawner, logger)
    {
    }

    public RandomSpawner(SpawnerLocationSelector locationSelector, EntitySpawner entitySpawner, ILogger<RandomSpawner> logger) : this(null, locationSelector, entitySpawner, logger)
    {
    }

    public RandomSpawner(Noise2D? noise, SpawnerLocationSelector? locationSelector, EntitySpawner entitySpawner, ILogger<RandomSpawner> logger)
    {
        _noise = noise;
        _locationSelector = locationSelector;
        _entitySpawner = entitySpawner;
        _logger = logger;
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
    public int? MaxSpawnPerExecution { get; set; } = 1;

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
        int fuel = 10000;
        while (suitableLocations.Count > 0 && (!maxSpawn.HasValue || totalSpawned < maxSpawn) && (!MaxCount.HasValue || _totalCount < MaxCount) && fuel > 0)
        {
            Location randomLocation = ChooseLocation(random, suitableLocations);

            bool wasEmpty = true;
            foreach (GameEntity entity in _entitySpawner.Spawn(randomLocation))
            {
                wasEmpty = false;
                yield return entity;
            }

            if (wasEmpty)
            {
                fuel--;
                continue;
            }

            if (!_countPerLocation.TryAdd(randomLocation, 1))
            {
                _countPerLocation[randomLocation] += 1;
            }
            totalSpawned++;
            _totalCount++;

            if (MaxCountPerLocation.HasValue && _countPerLocation[randomLocation] >= MaxCountPerLocation)
            {
                suitableLocations.Remove(randomLocation);
            }
        }

        if (fuel <= 0)
        {
            _logger.LogWarning("Spawner ran out of fuel");
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
        IEnumerable<Location> locations = _locationSelector?.GetLocations(state) ?? state.Content.Maps.Locations;

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

    Location ChooseLocation(Random random, HashSet<Location> suitableLocations)
    {
        if (_noise != null)
        {
            const int fuel = 1000;
            for (int i = 0; i < fuel; i++)
            {
                Location location = random.Choose(suitableLocations);
                double noise = _noise.Get(location.PositionX, location.PositionY);
                double check = random.NextDouble();
                if (check <= noise)
                {
                    return location;
                }
            }
        }

        return random.Choose(suitableLocations);
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
