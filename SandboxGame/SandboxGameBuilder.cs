using ContentToolbox.Spawners;
using ContentToolbox.Spawners.EntitySpawners;
using ContentToolbox.Spawners.LocationSelectors;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Plugins;
using RestAdventure.Core.Spawners;
using SandboxGame.Characters;
using SandboxGame.Generation;
using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Terraforming;
using SandboxGame.Generation.Zoning;
using SandboxGame.Jobs;
using SandboxGame.Monsters;
using SandboxGame.MyMath;

namespace SandboxGame;

public class SandboxGameBuilder
{
    readonly Location _startLocation;

    public SandboxGameBuilder(ILoggerFactory loggerFactory)
    {
        MapGenerator = new MapGenerator(
            new ErodedIslandGenerator(20, 20, 0.6),
            new VoronoiPartitionGenerator(5, loggerFactory.CreateLogger<VoronoiPartitionGenerator>()),
            new KingdomZonesGenerator(),
            loggerFactory.CreateLogger<MapGenerator>()
        );
        MapGeneratorResult = MapGenerator.Generate();

        (int X, int Y)? partitionCenter = MapGeneratorResult.Zones.MinBy(z => z.Level)?.PartitionCenter;
        Location? startLocation = partitionCenter == null
            ? null
            : MapGeneratorResult.GeneratedMaps.Locations.FirstOrDefault(l => l.PositionX == partitionCenter.Value.X && l.PositionY == partitionCenter.Value.Y);
        if (startLocation == null)
        {
            startLocation = MapGeneratorResult.GeneratedMaps.Locations.MinBy(l => Distance.L1((l.PositionX, l.PositionY), (0, 0)));
        }

        _startLocation = startLocation!;

        CharacterClasses = new CharacterClasses(MapGeneratorResult.GeneratedMaps, _startLocation);
        Rattlings = new Rattlings();
        Gatherer = new Gatherer();
    }

    public MapGenerator MapGenerator { get; }
    public MapGenerator.Result MapGeneratorResult { get; }
    public CharacterClasses CharacterClasses { get; }
    public Rattlings Rattlings { get; }
    public Gatherer Gatherer { get; }

    public Scenario Build()
    {
        Scenario scenario = new() { Name = "Rat Attack" };

        ExtractContent(scenario, CharacterClasses);
        ExtractContent(scenario, Rattlings);
        ExtractContent(scenario, Gatherer);
        ExtractContent(scenario, MapGeneratorResult.GeneratedMaps);

        scenario.Spawners.Add(new RandomSpawner(new WholeMapSpawnerLocationSelector(), new StaticObjectSpawner { StaticObject = Gatherer.AppleTree }) { MaxCount = 500 });
        scenario.Spawners.Add(
            new RandomSpawner(
                new WholeMapSpawnerLocationSelector(),
                new MonsterGroupSpawner
                {
                    Species = [Rattlings.PetitPaw, Rattlings.Rapierat, Rattlings.Biggaud, Rattlings.Melurat],
                    TeamSize = (1, 3),
                    LevelBounds = (1, 9)
                }
            ) { MaxCountPerLocation = 1, RespawnDelay = (5, 10) }
        );
        scenario.Spawners.Add(
            new RandomSpawner(
                new WholeMapSpawnerLocationSelector(),
                new MonsterGroupSpawner
                {
                    Species = [Rattlings.PetitPaw, Rattlings.Rapierat, Rattlings.Biggaud, Rattlings.Melurat],
                    TeamSize = (4, 6),
                    LevelBounds = (1, 9)
                }
            ) { MaxCountPerLocation = 1, RespawnDelay = (5, 10) }
        );
        scenario.Spawners.Add(
            new RandomSpawner(
                new WholeMapSpawnerLocationSelector(),
                new MonsterGroupSpawner
                {
                    Species = [Rattlings.PetitPaw, Rattlings.Rapierat, Rattlings.Biggaud, Rattlings.Melurat],
                    TeamSize = (7, 8),
                    LevelBounds = (1, 9)
                }
            ) { MaxCountPerLocation = 1, RespawnDelay = (5, 10) }
        );

        return scenario;
    }

    static void ExtractContent<T>(Scenario scenario, T instance)
    {
        scenario.CharacterClasses.AddRange(ObjectExplorer.FindValuesOfType<T, CharacterClass>(instance).ToArray());
        scenario.StaticObjects.AddRange(ObjectExplorer.FindValuesOfType<T, StaticObject>(instance).ToArray());
        scenario.Items.AddRange(ObjectExplorer.FindValuesOfType<T, Item>(instance).ToArray());
        scenario.Jobs.AddRange(ObjectExplorer.FindValuesOfType<T, Job>(instance).ToArray());
        scenario.Areas.AddRange(ObjectExplorer.FindValuesOfType<T, MapArea>(instance).ToArray());
        scenario.Locations.AddRange(ObjectExplorer.FindValuesOfType<T, Location>(instance).ToArray());
        scenario.Connections.AddRange(ObjectExplorer.FindValuesOfType<T, (Location, Location)>(instance).ToArray());
        scenario.Spawners.AddRange(ObjectExplorer.FindValuesOfType<T, Spawner>(instance).ToArray());
        scenario.MonsterFamilies.AddRange(ObjectExplorer.FindValuesOfType<T, MonsterFamily>(instance).ToArray());
        scenario.MonsterSpecies.AddRange(ObjectExplorer.FindValuesOfType<T, MonsterSpecies>(instance).ToArray());
    }
}
