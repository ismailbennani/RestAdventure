using Microsoft.Extensions.Logging;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Shaping;
using SandboxGame.Generation.Zoning;

namespace SandboxGame.Generation;

public class MapGenerator
{
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger<MapGenerator> _logger;

    public MapGenerator(LandGenerator landGenerator, PartitionGenerator partitionGenerator, ZonesGenerator zonesGenerator, ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<MapGenerator>();
        LandGenerator = landGenerator;
        PartitionGenerator = partitionGenerator;
        ZonesGenerator = zonesGenerator;
    }

    public LandGenerator LandGenerator { get; }
    public PartitionGenerator PartitionGenerator { get; }
    public ZonesGenerator ZonesGenerator { get; }

    public Result Generate()
    {
        Land land = LandGenerator.Generate();

        _logger.LogDebug("Land generation complete: {n} locations, [{xMin}, {yMin}] to [{xMax}, {yMax}]", land.Locations.Count, land.XMin, land.YMin, land.XMax, land.YMax);

        Partition partition = PartitionGenerator.Generate(land);

        _logger.LogDebug("Partition generation complete: {n} partition", partition.Count);

        IReadOnlyList<Zone> zones = ZonesGenerator.Generate(land, partition);

        _logger.LogDebug("Zones generation complete: {zones}", string.Join(", ", zones.Select(z => $"{z.Name} (lv. {z.Level})")));

        return new Result
        {
            Land = land,
            Partition = partition,
            Zones = zones,
            GeneratedMaps = Generate(land, partition, zones)
        };
    }

    GeneratedMaps Generate(Land land, Partition partition, IReadOnlyList<Zone> zones)
    {
        MapArea noArea = new() { Name = "Void", Level = 0 };
        List<MapArea> areas = GenerateAreas(zones).ToList();
        IReadOnlyCollection<Location> locations = GenerateLocations(land, partition, zones, areas, noArea, out bool atLeastOneLocationInNoArea);
        IReadOnlyCollection<(Location, Location)> connections = GenerateConnections(locations).ToArray();

        if (atLeastOneLocationInNoArea)
        {
            _logger.LogWarning("Some locations were not zoned, they were assigned to a default area called 'Void'");
            areas.Add(noArea);
        }

        return new GeneratedMaps
        {
            Areas = areas,
            Locations = locations,
            Connections = connections,
            Spawners = []
        };
    }

    static IEnumerable<MapArea> GenerateAreas(IEnumerable<Zone> zones) => zones.Select(z => new MapArea { Name = z.Name, Level = z.Level });

    static IReadOnlyCollection<Location> GenerateLocations(
        Land land,
        Partition partition,
        IReadOnlyList<Zone> zones,
        IReadOnlyList<MapArea> areas,
        MapArea noArea,
        out bool atLeastOneLocationInNoArea
    )
    {
        MapArea[] areasOrderedByPartition = zones.Select((z, i) => new { Area = areas[i], z.PartitionIndex }).OrderBy(x => x.PartitionIndex).Select(x => x.Area).ToArray();

        atLeastOneLocationInNoArea = false;
        List<Location> locations = [];
        foreach ((int X, int Y) position in land.Locations)
        {
            if (partition.TryGetSubset(position, out int positionPartition))
            {
                locations.Add(new Location { Area = areasOrderedByPartition[positionPartition], PositionX = position.X, PositionY = position.Y });
            }
            else
            {
                locations.Add(new Location { Area = noArea, PositionX = position.X, PositionY = position.Y });
                atLeastOneLocationInNoArea = true;
            }
        }

        return locations;
    }

    static IEnumerable<(Location, Location)> GenerateConnections(IReadOnlyCollection<Location> locations)
    {
        foreach (Location location in locations)
        {
            Location? top = locations.FirstOrDefault(l => l.PositionX == location.PositionX && l.PositionY == location.PositionY + 1);
            if (top != null)
            {
                yield return (location, top);
            }
            Location? right = locations.FirstOrDefault(l => l.PositionX == location.PositionX + 1 && l.PositionY == location.PositionY);
            if (right != null)
            {
                yield return (location, right);
            }
        }
    }

    public class Result
    {
        public required Land Land { get; init; }
        public required Partition Partition { get; init; }
        public required IReadOnlyList<Zone> Zones { get; init; }
        public required GeneratedMaps GeneratedMaps { get; init; }
    }
}
