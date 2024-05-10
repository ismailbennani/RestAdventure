using Microsoft.Extensions.Logging;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Terraforming;

namespace SandboxGame.Generation;

public class MapGenerator
{
    readonly ILogger<MapGenerator> _logger;

    public MapGenerator(LandGenerator landGenerator, PartitionGenerator partitionGenerator, ILogger<MapGenerator> logger)
    {
        _logger = logger;
        LandGenerator = landGenerator;
        PartitionGenerator = partitionGenerator;
    }

    public LandGenerator LandGenerator { get; }
    public PartitionGenerator PartitionGenerator { get; }

    public GeneratedMaps Generate()
    {
        Land land = LandGenerator.Generate();

        _logger.LogDebug("Land generation complete: {n} locations, [{xMin}, {yMin}] to [{xMax}, {yMax}]", land.Locations.Count, land.XMin, land.YMin, land.XMax, land.YMax);

        Partition partition = PartitionGenerator.Generate(land);

        _logger.LogDebug("Partition generation complete: {n} partition", partition.Count);

        return Generate(land, partition);
    }

    GeneratedMaps Generate(Land land, Partition partition)
    {
        MapArea noArea = new() { Name = "Void", Level = 0 };
        List<MapArea> areas = GenerateAreas(partition).ToList();
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

    static IEnumerable<MapArea> GenerateAreas(Partition partition) => Enumerable.Range(0, partition.Count).Select(i => new MapArea { Name = "Zone " + i, Level = 0 });

    public IReadOnlyCollection<Location> GenerateLocations(Partition partition, Land land, IReadOnlyList<MapArea> areas, MapArea noArea, out bool atLeastOneLocationInNoArea)
    {
        atLeastOneLocationInNoArea = false;
        List<Location> locations = new();
        foreach ((int x, int y) in land.Locations)
        {
            int? zone = partition.GetSubsetContaining((x, y));
            if (zone.HasValue)
            {
                locations.Add(new Location { Area = areas[zone.Value], PositionX = x, PositionY = y });
            }
            else
            {
                locations.Add(new Location { Area = noArea, PositionX = x, PositionY = y });
                atLeastOneLocationInNoArea = true;
            }
        }

        return locations;
    }

    IEnumerable<(Location, Location)> GenerateConnections(IReadOnlyCollection<Location> locations)
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
