using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Extensions;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Spawners.EntitySpawners;

public class MonsterGroupByAreaLevelSpawner : EntitySpawner
{
    /// <summary>
    ///     The species that should be chosen from
    /// </summary>
    public required IReadOnlyCollection<(int MinLevel, int MaxLevel, IReadOnlyCollection<MonsterSpecies> Species)> SpeciesByLevelRange { get; init; }

    /// <summary>
    ///     The number of monsters per team
    /// </summary>
    public required (int Min, int Max) TeamSize { get; init; }

    public override IEnumerable<GameEntity> Spawn(Location location)
    {
        (int MinLevel, int MaxLevel, IReadOnlyCollection<MonsterSpecies> Species)? speciesAndBounds = GetSpecies(location.Area.Level);
        if (!speciesAndBounds.HasValue)
        {
            yield break;
        }

        Random random = Random.Shared;

        int size = random.Next(TeamSize.Min, TeamSize.Max + 1);
        List<MonsterInGroup> group = [];
        for (int i = 0; i < size; i++)
        {
            MonsterSpecies species = random.Choose(speciesAndBounds.Value.Species);
            int level = random.Next(speciesAndBounds.Value.MinLevel, speciesAndBounds.Value.MaxLevel + 1);
            group.Add(new MonsterInGroup { Species = species, Level = level });
        }

        yield return new MonsterGroup(group, location);
    }

    (int MinLevel, int MaxLevel, IReadOnlyCollection<MonsterSpecies> Species)? GetSpecies(int level)
    {
        foreach ((int MinLevel, int MaxLevel, IReadOnlyCollection<MonsterSpecies> Species) value in SpeciesByLevelRange)
        {
            if (level >= value.MinLevel && level <= value.MaxLevel)
            {
                return value;
            }
        }

        return null;
    }
}
