using Microsoft.Extensions.Logging;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Terraforming;
using SandboxGame.Generation.Zoning;

namespace SandboxGame.Generation;

public class MapGenerator
{
    readonly ILogger<MapGenerator> _logger;

    public MapGenerator(LandGenerator landGenerator, PartitionGenerator partitionGenerator, ZonesGenerator zonesGenerator, ILogger<MapGenerator> logger)
    {
        _logger = logger;
        LandGenerator = landGenerator;
        PartitionGenerator = partitionGenerator;
        ZonesGenerator = zonesGenerator;
    }

    public LandGenerator LandGenerator { get; }
    public PartitionGenerator PartitionGenerator { get; }
    public ZonesGenerator ZonesGenerator { get; }

    public GeneratedMaps Generate()
    {
        Land land = LandGenerator.Generate();

        _logger.LogDebug("Land generation complete: {n} locations, [{xMin}, {yMin}] to [{xMax}, {yMax}]", land.Locations.Count, land.XMin, land.YMin, land.XMax, land.YMax);

        Partition partition = PartitionGenerator.Generate(land);

        _logger.LogDebug("Partition generation complete: {n} partition", partition.Count);

        IReadOnlyList<Zone> zones = ZonesGenerator.Generate(land, partition);

        _logger.LogDebug("Zones generation complete: {zones}", string.Join(", ", zones.Select(z => $"{z.Name} (lv. {z.Level})")));

        return Generate(land, partition, zones);
    }

    GeneratedMaps Generate(Land land, Partition partition, IReadOnlyList<Zone> zones)
    {
        MapArea noArea = new() { Name = "Void", Level = 0 };
        List<MapArea> areas = GenerateAreas(zones).ToList();
        IReadOnlyCollection<Location> locations = GenerateLocations(partition, land, areas, noArea, out bool atLeastOneLocationInNoArea);
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

    static IReadOnlyCollection<Location> GenerateLocations(Partition partition, Land land, IReadOnlyList<MapArea> areas, MapArea noArea, out bool atLeastOneLocationInNoArea)
    {
        atLeastOneLocationInNoArea = false;
        List<Location> locations = [];
        foreach ((int X, int Y) position in land.Locations)
        {
            if (partition.TryGetSubset(position, out int positionPartition))
            {
                locations.Add(new Location { Area = areas[positionPartition], PositionX = position.X, PositionY = position.Y });
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
}
