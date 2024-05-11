using ContentToolbox.Noise;
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
using SandboxGame.Generation.Shaping;
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
        Rattlings = new Rattlings();
        Forester = new Forester();
        Herbalist = new Herbalist();

        IReadOnlyCollection<NoiseResourceAllocationGenerator.WeightedResource> herbalistLeveledRepartition =
        [
            new NoiseResourceAllocationGenerator.WeightedResource { Object = Herbalist.PeppermintPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 10, 5 } } },
            new NoiseResourceAllocationGenerator.WeightedResource { Object = Herbalist.LavenderPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0 }, { 20, 4 } } },
            new NoiseResourceAllocationGenerator.WeightedResource { Object = Herbalist.GinsengPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0 }, { 30, 3 } } },
            new NoiseResourceAllocationGenerator.WeightedResource { Object = Herbalist.ChamomilePlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0 }, { 40, 2 } } },
            new NoiseResourceAllocationGenerator.WeightedResource
            {
                Object = Herbalist.EchinaceaPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0 }, { 50, 1 } }
            }
        ];

        IReadOnlyCollection<NoiseResourceAllocationGenerator.WeightedResource> foresterLeveledRepartition =
        [
            new NoiseResourceAllocationGenerator.WeightedResource { Object = Forester.OakTree, WeightsByZoneLevel = new Dictionary<int, double> { { 10, 16 } } },
            new NoiseResourceAllocationGenerator.WeightedResource { Object = Forester.PineTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0 }, { 20, 8 } } },
            new NoiseResourceAllocationGenerator.WeightedResource { Object = Forester.MapleTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0 }, { 30, 4 } } },
            new NoiseResourceAllocationGenerator.WeightedResource { Object = Forester.BirchTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0 }, { 40, 2 } } },
            new NoiseResourceAllocationGenerator.WeightedResource
            {
                Object = Forester.WalnutTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0 }, { 50, 1 } }
            }
        ];

        MapGenerator = new MapGenerator(
            new ErodedIslandGenerator(100, 100, 0.6),
            new VoronoiPartitionGenerator(100, loggerFactory.CreateLogger<VoronoiPartitionGenerator>()),
            new KingdomZonesGenerator(),
            [
                new NoiseResourceAllocationGenerator(herbalistLeveledRepartition, new SimplexNoise2D(0.05f)) { Coefficient = 1 },
                new NoiseResourceAllocationGenerator(foresterLeveledRepartition, new PerlinNoise2D(0.05f)) { Coefficient = 0.5f, NoiseCutoff = 0.6 },
                new NoiseResourceAllocationGenerator(foresterLeveledRepartition, new PerlinNoise2D(0.1f)) { Coefficient = 2, NoiseCutoff = 0.7 }
            ],
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
    }

    public MapGenerator MapGenerator { get; }
    public MapGenerator.Result MapGeneratorResult { get; }
    public CharacterClasses CharacterClasses { get; }
    public Rattlings Rattlings { get; }
    public Forester Forester { get; }
    public Herbalist Herbalist { get; }

    public Scenario Build()
    {
        Scenario scenario = new() { Name = "Rat Attack" };

        ExtractContent(scenario, CharacterClasses);
        ExtractContent(scenario, Rattlings);
        ExtractContent(scenario, Forester);
        ExtractContent(scenario, Herbalist);
        ExtractContent(scenario, MapGeneratorResult.GeneratedMaps);

        scenario.Spawners.Add(
            new RandomSpawner(
                new MapAreaSpawnerLocationSelector { Area = _startLocation.Area },
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
                new MapAreaSpawnerLocationSelector { Area = _startLocation.Area },
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
                new MapAreaSpawnerLocationSelector { Area = _startLocation.Area },
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
