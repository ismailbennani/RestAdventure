using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Spawners;

public class AreaMonstersSpawner : Spawner
{
    public AreaMonstersSpawner(MapArea area, IReadOnlyCollection<MonsterSpecies> species, (int Min, int Max) teamSize, (int Min, int Max) levelBounds)
    {
        Area = area;
        Species = species;
        TeamSize = teamSize;
        LevelBounds = levelBounds;
    }

    public MapArea Area { get; }
    public IReadOnlyCollection<MonsterSpecies> Species { get; }
    public (int Min, int Max) TeamSize { get; }
    public (int Min, int Max) LevelBounds { get; }

    public override IEnumerable<GameEntity> GetEntitiesToSpawn(GameState state) => state.Content.Maps.Locations.InArea(Area).SelectMany(l => GetEntityToSpawnAt(state, l));

    IEnumerable<GameEntity> GetEntityToSpawnAt(GameState state, Location location)
    {
        if (state.Entities.AtLocation<MonsterInstance>(location).Any(m => m.Source is AreaMonstersSpawner spawner && spawner == this))
        {
            yield break;
        }

        Random random = Random.Shared;

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
