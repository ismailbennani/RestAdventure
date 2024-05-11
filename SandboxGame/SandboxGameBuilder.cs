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

        MapGenerator = new MapGenerator(
            new ErodedIslandGenerator(100, 100, 0.6),
            new VoronoiPartitionGenerator(50, loggerFactory.CreateLogger<VoronoiPartitionGenerator>()),
            new KingdomZonesGenerator(),
            [
                new ForestResourceAllocationGenerator(
                    [
                        new ForestResourceAllocationGenerator.WeightedResource
                            { Object = Forester.OakTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0.01 }, { 9, 0 }, { 10, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource
                            { Object = Forester.PineTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0.01 }, { 9, 0 }, { 20, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource
                            { Object = Forester.MapleTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0.01 }, { 9, 0 }, { 30, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource
                            { Object = Forester.BirchTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0.01 }, { 9, 0 }, { 40, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource
                        {
                            Object = Forester.WalnutTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0.01 }, { 9, 0 }, { 50, 1 } }
                        }
                    ]
                ) { ForestDensity = 5, ForestSize = 4 },
                new ForestResourceAllocationGenerator(
                    [
                        new ForestResourceAllocationGenerator.WeightedResource { Object = Forester.OakTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource { Object = Forester.PineTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource { Object = Forester.MapleTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource { Object = Forester.BirchTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource { Object = Forester.WalnutTree, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 1 } } }
                    ]
                ) { ForestDensity = 1, ForestSize = 10, DistanceCutoff = 30 },
                new MultiForestResourceAllocationGenerator(
                    new ForestResourceAllocationGenerator(
                        [
                            new ForestResourceAllocationGenerator.WeightedResource
                                { Object = Herbalist.PeppermintPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0.01 }, { 9, 0 }, { 10, 1 } } },
                            new ForestResourceAllocationGenerator.WeightedResource
                                { Object = Herbalist.LavenderPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0.01 }, { 9, 0 }, { 20, 1 } } },
                            new ForestResourceAllocationGenerator.WeightedResource
                                { Object = Herbalist.GinsengPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0.01 }, { 9, 0 }, { 30, 1 } } },
                            new ForestResourceAllocationGenerator.WeightedResource
                                { Object = Herbalist.ChamomilePlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0.01 }, { 9, 0 }, { 40, 1 } } },
                            new ForestResourceAllocationGenerator.WeightedResource
                            {
                                Object = Herbalist.EchinaceaPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 0.01 }, { 9, 0 }, { 50, 1 } }
                            }
                        ]
                    ) { ForestDensity = 5, ForestSize = 5, DistanceCutoff = 10 },
                    3
                ),
                new ForestResourceAllocationGenerator(
                    [
                        new ForestResourceAllocationGenerator.WeightedResource
                        {
                            Object = Herbalist.PeppermintPlant,
                            WeightsByZoneLevel = new Dictionary<int, double> { { 0, 1 } }
                        },
                        new ForestResourceAllocationGenerator.WeightedResource { Object = Herbalist.LavenderPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource { Object = Herbalist.GinsengPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource { Object = Herbalist.ChamomilePlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 1 } } },
                        new ForestResourceAllocationGenerator.WeightedResource { Object = Herbalist.EchinaceaPlant, WeightsByZoneLevel = new Dictionary<int, double> { { 0, 1 } } }
                    ]
                ) { ForestDensity = 0.5, ForestSize = 10, DistanceCutoff = 50 }
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
