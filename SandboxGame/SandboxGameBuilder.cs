﻿using ContentToolbox.Noise;
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
using RestAdventure.Core.Utils;
using SandboxGame.Characters;
using SandboxGame.Generation;
using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Shaping;
using SandboxGame.Generation.Zoning;
using SandboxGame.Items;
using SandboxGame.Jobs;
using SandboxGame.Monsters;

namespace SandboxGame;

public class SandboxGameBuilder
{
    readonly ILoggerFactory _loggerFactory;
    readonly Location _startLocation;

    public SandboxGameBuilder(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        ResourceCategories = new ResourceCategories();
        WeaponCategories = new WeaponCategories();
        Whimsicals = new Whimsicals(ResourceCategories);
        Rattlings = new Rattlings(ResourceCategories);
        Forester = new Forester(ResourceCategories, WeaponCategories);
        Herbalist = new Herbalist(ResourceCategories);
        Miner = new Miner(ResourceCategories, WeaponCategories);

        MapGenerator = new MapGenerator(
            new ErodedIslandGenerator(60, 60, 0.6),
            new VoronoiPartitionGenerator(25, loggerFactory.CreateLogger<VoronoiPartitionGenerator>()),
            new KingdomZonesGenerator(),
            loggerFactory
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

        CharacterClasses = new CharacterClasses(_startLocation);
    }

    public MapGenerator MapGenerator { get; }
    public MapGenerator.Result MapGeneratorResult { get; }
    public ResourceCategories ResourceCategories { get; }
    public WeaponCategories WeaponCategories { get; }
    public CharacterClasses CharacterClasses { get; }
    public Whimsicals Whimsicals { get; }
    public Rattlings Rattlings { get; }
    public Forester Forester { get; }
    public Herbalist Herbalist { get; }
    public Miner Miner { get; }

    public Scenario Build()
    {
        Scenario scenario = new() { Name = "Rat Attack" };

        ExtractContent(scenario, CharacterClasses);
        ExtractContent(scenario, Whimsicals);
        ExtractContent(scenario, Rattlings);
        ExtractContent(scenario, Forester);
        ExtractContent(scenario, Herbalist);
        ExtractContent(scenario, Miner);
        ExtractContent(scenario, MapGeneratorResult.GeneratedMaps);

        Random random = Random.Shared;

        foreach (MapArea area in scenario.Areas)
        {
            if (_startLocation.Area == area)
            {
                scenario.Spawners.Add(GetMonsterSpawners(area, area.Level, (1, 2), [Whimsicals.PetitPaw, Whimsicals.Bumblebun, Whimsicals.Flutterfly, Whimsicals.Chirpchirp]));
                scenario.Spawners.Add(GetMonsterSpawners(area, area.Level, (2, 4), [Whimsicals.PetitPaw, Whimsicals.Bumblebun, Whimsicals.Flutterfly, Whimsicals.Chirpchirp]));
            }
            else
            {
                scenario.Spawners.Add(GetMonsterSpawners(area, area.Level, (1, 3), [Rattlings.Fantorat, Rattlings.Rapierat, Rattlings.Biggaud, Rattlings.Melurat]));
                scenario.Spawners.Add(GetMonsterSpawners(area, area.Level, (4, 6), [Rattlings.Fantorat, Rattlings.Rapierat, Rattlings.Biggaud, Rattlings.Melurat]));
                scenario.Spawners.Add(GetMonsterSpawners(area, area.Level, (7, 8), [Rattlings.Fantorat, Rattlings.Rapierat, Rattlings.Biggaud, Rattlings.Melurat]));
            }
        }

        scenario.Spawners.AddRange(GetHerbalistSpawners(random));
        scenario.Spawners.AddRange(GetForesterSpawners(random));
        scenario.Spawners.AddRange(GetMinerSpawners(random));

        return scenario;
    }

    static void ExtractContent<T>(Scenario scenario, T instance)
    {
        scenario.CharacterClasses.AddRange(ObjectExplorer.FindValuesOfType<T, CharacterClass>(instance).ToArray());
        scenario.StaticObjects.AddRange(ObjectExplorer.FindValuesOfType<T, StaticObject>(instance).ToArray());
        scenario.ItemCategories.AddRange(ObjectExplorer.FindValuesOfType<T, ItemCategory>(instance).ToArray());
        scenario.Items.AddRange(ObjectExplorer.FindValuesOfType<T, Item>(instance).ToArray());
        scenario.Jobs.AddRange(ObjectExplorer.FindValuesOfType<T, Job>(instance).ToArray());
        scenario.Areas.AddRange(ObjectExplorer.FindValuesOfType<T, MapArea>(instance).ToArray());
        scenario.Locations.AddRange(ObjectExplorer.FindValuesOfType<T, Location>(instance).ToArray());
        scenario.Connections.AddRange(ObjectExplorer.FindValuesOfType<T, (Location, Location)>(instance).ToArray());
        scenario.Spawners.AddRange(ObjectExplorer.FindValuesOfType<T, Spawner>(instance).ToArray());
        scenario.MonsterFamilies.AddRange(ObjectExplorer.FindValuesOfType<T, MonsterFamily>(instance).ToArray());
        scenario.MonsterSpecies.AddRange(ObjectExplorer.FindValuesOfType<T, MonsterSpecies>(instance).ToArray());
    }

    IEnumerable<RandomSpawner> GetHerbalistSpawners(Random random)
    {
        yield return new RandomSpawner(
            new PerlinNoise2D(random.Next(), 0.01f) { LowCutoff = 0.5 },
            new RandomByAreaLevelStaticObjectSpawner(
                new Dictionary<int, IReadOnlyCollection<Weighted<StaticObject>>>
                {
                    {
                        0, [
                            new Weighted<StaticObject>(Herbalist.PeppermintPlant, 1)
                        ]
                    },
                    {
                        10, [
                            new Weighted<StaticObject>(Herbalist.PeppermintPlant, 2),
                            new Weighted<StaticObject>(Herbalist.LavenderPlant, 1)
                        ]
                    },
                    {
                        20, [
                            new Weighted<StaticObject>(Herbalist.PeppermintPlant, 3),
                            new Weighted<StaticObject>(Herbalist.LavenderPlant, 2),
                            new Weighted<StaticObject>(Herbalist.GinsengPlant, 1)
                        ]
                    },
                    {
                        30, [
                            new Weighted<StaticObject>(Herbalist.PeppermintPlant, 4),
                            new Weighted<StaticObject>(Herbalist.LavenderPlant, 3),
                            new Weighted<StaticObject>(Herbalist.GinsengPlant, 2),
                            new Weighted<StaticObject>(Herbalist.ChamomilePlant, 1)
                        ]
                    },
                    {
                        40, [
                            new Weighted<StaticObject>(Herbalist.PeppermintPlant, 5),
                            new Weighted<StaticObject>(Herbalist.LavenderPlant, 4),
                            new Weighted<StaticObject>(Herbalist.GinsengPlant, 3),
                            new Weighted<StaticObject>(Herbalist.ChamomilePlant, 2),
                            new Weighted<StaticObject>(Herbalist.EchinaceaPlant, 1)
                        ]
                    }
                }
            ),
            _loggerFactory.CreateLogger<RandomSpawner>()
        ) { MaxCount = 1000, MaxCountPerLocation = 10, MaxSpawnPerExecution = 10 };
    }

    IEnumerable<RandomSpawner> GetForesterSpawners(Random random)
    {
        yield return new RandomSpawner(
            new SimplexNoise2D(random.Next(), 0.1f) { LowCutoff = 0.75 },
            new RandomByAreaLevelStaticObjectSpawner(
                new Dictionary<int, IReadOnlyCollection<Weighted<StaticObject>>>
                {
                    {
                        0, [
                            new Weighted<StaticObject>(Forester.OakTree, 1)
                        ]
                    },
                    {
                        10, [
                            new Weighted<StaticObject>(Forester.OakTree, 2),
                            new Weighted<StaticObject>(Forester.PineTree, 1)
                        ]
                    },
                    {
                        20, [
                            new Weighted<StaticObject>(Forester.OakTree, 4),
                            new Weighted<StaticObject>(Forester.PineTree, 2),
                            new Weighted<StaticObject>(Forester.MapleTree, 1)
                        ]
                    },
                    {
                        30, [
                            new Weighted<StaticObject>(Forester.OakTree, 8),
                            new Weighted<StaticObject>(Forester.PineTree, 4),
                            new Weighted<StaticObject>(Forester.MapleTree, 2),
                            new Weighted<StaticObject>(Forester.BirchTree, 1)
                        ]
                    },
                    {
                        40, [
                            new Weighted<StaticObject>(Forester.OakTree, 16),
                            new Weighted<StaticObject>(Forester.PineTree, 8),
                            new Weighted<StaticObject>(Forester.MapleTree, 4),
                            new Weighted<StaticObject>(Forester.BirchTree, 2),
                            new Weighted<StaticObject>(Forester.WalnutTree, 1)
                        ]
                    }
                }
            ),
            _loggerFactory.CreateLogger<RandomSpawner>()
        ) { MaxCount = 500, MaxCountPerLocation = 5, MaxSpawnPerExecution = 10 };
    }

    IEnumerable<RandomSpawner> GetMinerSpawners(Random random)
    {
        yield return new RandomSpawner(
            new PerlinNoise2D(random.Next(), 0.05f) { LowCutoff = 0.75 },
            new RandomByAreaLevelStaticObjectSpawner(
                new Dictionary<int, IReadOnlyCollection<Weighted<StaticObject>>>
                {
                    {
                        0, [
                            new Weighted<StaticObject>(Miner.IronOre, 1)
                        ]
                    },
                    {
                        10, [
                            new Weighted<StaticObject>(Miner.IronOre, 2),
                            new Weighted<StaticObject>(Miner.CopperOre, 1)
                        ]
                    },
                    {
                        20, [
                            new Weighted<StaticObject>(Miner.IronOre, 4),
                            new Weighted<StaticObject>(Miner.CopperOre, 2),
                            new Weighted<StaticObject>(Miner.SilverOre, 1)
                        ]
                    },
                    {
                        30, [
                            new Weighted<StaticObject>(Miner.IronOre, 8),
                            new Weighted<StaticObject>(Miner.CopperOre, 4),
                            new Weighted<StaticObject>(Miner.SilverOre, 2),
                            new Weighted<StaticObject>(Miner.GoldOre, 1)
                        ]
                    },
                    {
                        40, [
                            new Weighted<StaticObject>(Miner.IronOre, 16),
                            new Weighted<StaticObject>(Miner.CopperOre, 8),
                            new Weighted<StaticObject>(Miner.SilverOre, 4),
                            new Weighted<StaticObject>(Miner.GoldOre, 2),
                            new Weighted<StaticObject>(Miner.MithrilOre, 1)
                        ]
                    }
                }
            ),
            _loggerFactory.CreateLogger<RandomSpawner>()
        ) { MaxCount = 100, MaxCountPerLocation = 3, MaxSpawnPerExecution = 10 };
    }

    RandomSpawner GetMonsterSpawners(MapArea area, int level, (int, int) teamSize, IReadOnlyList<MonsterSpecies> monsters)
    {
        (int, int) respawnDelay = (5, 10);

        switch (level)
        {
            case 0:
                return new RandomSpawner(
                    new MapAreaSpawnerLocationSelector { Area = area },
                    new MonsterGroupSpawner
                    {
                        Species = monsters,
                        TeamSize = teamSize,
                        LevelBounds = (0, 9)
                    },
                    _loggerFactory.CreateLogger<RandomSpawner>()
                ) { MaxCountPerLocation = 1, RespawnDelay = respawnDelay };
            case 10:
                return new RandomSpawner(
                    new MapAreaSpawnerLocationSelector { Area = area },
                    new MonsterGroupSpawner
                    {
                        Species = monsters,
                        TeamSize = teamSize,
                        LevelBounds = (10, 19)
                    },
                    _loggerFactory.CreateLogger<RandomSpawner>()
                ) { MaxCountPerLocation = 1, RespawnDelay = respawnDelay };
            case 20:
                return new RandomSpawner(
                    new MapAreaSpawnerLocationSelector { Area = area },
                    new MonsterGroupSpawner
                    {
                        Species = monsters,
                        TeamSize = teamSize,
                        LevelBounds = (20, 29)
                    },
                    _loggerFactory.CreateLogger<RandomSpawner>()
                ) { MaxCountPerLocation = 1, RespawnDelay = respawnDelay };
            case 30:
                return new RandomSpawner(
                    new MapAreaSpawnerLocationSelector { Area = area },
                    new MonsterGroupSpawner
                    {
                        Species = monsters,
                        TeamSize = teamSize,
                        LevelBounds = (30, 39)
                    },
                    _loggerFactory.CreateLogger<RandomSpawner>()
                ) { MaxCountPerLocation = 1, RespawnDelay = respawnDelay };
            case 40:
                return new RandomSpawner(
                    new MapAreaSpawnerLocationSelector { Area = area },
                    new MonsterGroupSpawner
                    {
                        Species = monsters,
                        TeamSize = teamSize,
                        LevelBounds = (40, 49)
                    },
                    _loggerFactory.CreateLogger<RandomSpawner>()
                ) { MaxCountPerLocation = 1, RespawnDelay = respawnDelay };
            case 50:
                return new RandomSpawner(
                    new MapAreaSpawnerLocationSelector { Area = area },
                    new MonsterGroupSpawner
                    {
                        Species = monsters,
                        TeamSize = teamSize,
                        LevelBounds = (50, 59)
                    },
                    _loggerFactory.CreateLogger<RandomSpawner>()
                ) { MaxCountPerLocation = 1, RespawnDelay = respawnDelay };
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
}
