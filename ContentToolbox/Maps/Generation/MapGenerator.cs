using ContentToolbox.Maps.Generation.LandGeneration;
using ContentToolbox.Maps.Generation.Zoning;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Maps.Generation;

public class MapGenerator
{
    readonly ILogger<MapGenerator> _logger;

    public MapGenerator(LandGenerator landGenerator, ZonesGenerator zonesGenerator, ILogger<MapGenerator> logger)
    {
        _logger = logger;
        LandGenerator = landGenerator;
        ZonesGenerator = zonesGenerator;
    }

    public LandGenerator LandGenerator { get; }
    public ZonesGenerator ZonesGenerator { get; }

    public GeneratedMaps Generate()
    {
        Land land = LandGenerator.Generate();

        _logger.LogDebug("Land generation complete: {n} locations, [{xMin}, {yMin}] to [{xMax}, {yMax}]", land.Locations.Count, land.XMin, land.YMin, land.XMax, land.YMax);

        Zones zones = ZonesGenerator.Generate(land);

        _logger.LogDebug("Zones generation complete: {n} zones", zones.Count);

        MapArea noArea = new() { Name = "Void" };
        List<MapArea> areas = Enumerable.Range(0, zones.Count).Select(i => new MapArea { Name = "Zone " + i }).ToList();

        bool atLeastOneLocationInNoArea = false;
        List<Location> locations = new();
        foreach ((int x, int y) in land.Locations)
        {
            int? zone = zones.GetZone((x, y));
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

        if (atLeastOneLocationInNoArea)
        {
            _logger.LogWarning("Some locations were not zoned, they were assigned to a default area called 'Void'");
            areas.Add(noArea);
        }

        return new GeneratedMaps
        {
            Areas = areas,
            Locations = locations,
            Connections = [],
            Spawners = []
        };
    }
}
