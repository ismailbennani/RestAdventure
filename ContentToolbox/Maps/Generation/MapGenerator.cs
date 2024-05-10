using ContentToolbox.Maps.Generation.Land;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Maps.Generation;

public class MapGenerator
{
    readonly ILogger<MapGenerator> _logger;

    public MapGenerator(LandGenerator landGenerator, ILogger<MapGenerator> logger)
    {
        _logger = logger;
        LandGenerator = landGenerator;
    }

    public LandGenerator LandGenerator { get; }

    public GeneratedMaps Generate()
    {
        Land.Land land = LandGenerator.Generate();

        _logger.LogDebug("Land generation complete: {n} locations, [{xMin}, {yMin}] to [{xMax}, {yMax}]", land.Locations.Count, land.XMin, land.YMin, land.XMax, land.YMax);

        MapArea area = new() { Name = "Start" };

        return new GeneratedMaps
        {
            Areas = [area],
            Locations = land.Locations.Select(pos => new Location { Area = area, PositionX = pos.X, PositionY = pos.Y }).ToArray(),
            Connections = [],
            Spawners = []
        };
    }
}
