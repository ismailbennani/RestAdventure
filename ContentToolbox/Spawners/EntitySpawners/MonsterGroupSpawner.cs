using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Spawners.EntitySpawners;

public class MonsterGroupSpawner : EntitySpawner, IGameEntitySource
{
    /// <summary>
    ///     The species that should be sampled for each instance
    /// </summary>
    public required IReadOnlyCollection<MonsterSpecies> Species { get; init; }

    /// <summary>
    ///     The number of monsters per team
    /// </summary>
    public required (int Min, int Max) TeamSize { get; init; }

    /// <summary>
    ///     The level of each monster
    /// </summary>
    public required (int Min, int Max) LevelBounds { get; init; }

    public override IEnumerable<GameEntity> Spawn(Location location)
    {
        Random random = Random.Shared;

        int size = random.Next(TeamSize.Min, TeamSize.Max + 1);
        List<MonsterInGroup> group = [];
        for (int i = 0; i < size; i++)
        {
            MonsterSpecies species = random.Choose(Species);
            int level = random.Next(LevelBounds.Min, LevelBounds.Max + 1);
            group.Add(new MonsterInGroup { Species = species, Level = level });
        }

        yield return new MonsterGroup(group, location) { Source = this };
    }
}
