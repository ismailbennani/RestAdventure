using RestAdventure.Core;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Spawners;

namespace ContentToolbox.Spawners;

/// <summary>
///     Spawn a monster group at each location of an area
/// </summary>
public class AreaMonstersSpawner : Spawner
{
    public AreaMonstersSpawner(MapArea area, IReadOnlyCollection<MonsterSpecies> species, (int Min, int Max) teamSize, (int Min, int Max) levelBounds)
    {
        Area = area;
        Species = species;
        TeamSize = teamSize;
        LevelBounds = levelBounds;
    }

    /// <summary>
    ///     The area to spawn the monsters in
    /// </summary>
    public MapArea Area { get; }

    /// <summary>
    ///     The species that should be sampled for each instance
    /// </summary>
    public IReadOnlyCollection<MonsterSpecies> Species { get; }

    /// <summary>
    ///     The number of monsters per team
    /// </summary>
    public (int Min, int Max) TeamSize { get; }

    /// <summary>
    ///     The level of each monster
    /// </summary>
    public (int Min, int Max) LevelBounds { get; }

    /// <summary>
    ///     The max number of monster groups spawned per execution of the spawner
    /// </summary>
    public int? MaxGroupsSpawnedPerExecution { get; init; }

    public override IEnumerable<GameEntity> GetInitialEntities(GameState state) => state.Content.Maps.Locations.InArea(Area).SelectMany(l => SpawnEntities(Random.Shared, l));

    /// <summary>
    ///     Spawn a team of monster per location of the area where the previous group has disappeared
    /// </summary>
    public override IEnumerable<GameEntity> GetEntitiesToSpawn(GameState state)
    {
        int groupsSpawned = 0;
        foreach (Location location in state.Content.Maps.Locations.InArea(Area))
        {
            if (state.Entities.AtLocation<MonsterInstance>(location).Any(m => m.Source is AreaMonstersSpawner spawner && spawner == this))
            {
                continue;
            }

            IReadOnlyCollection<GameEntity> spawned = SpawnEntities(Random.Shared, location).ToArray();

            if (spawned.Count > 0)
            {
                foreach (GameEntity entity in spawned)
                {
                    yield return entity;
                }

                groupsSpawned += 1;

                if (groupsSpawned >= MaxGroupsSpawnedPerExecution)
                {
                    break;
                }
            }
        }
    }

    IEnumerable<GameEntity> SpawnEntities(Random random, Location location)
    {
        int size = random.Next(TeamSize.Min, TeamSize.Max + 1);
        Team team = new();

        for (int i = 0; i < size; i++)
        {
            MonsterSpecies species = random.Choose(Species);
            int level = random.Next(LevelBounds.Min, LevelBounds.Max + 1);
            yield return new MonsterInstance(team, species, level, location);
        }
    }
}
