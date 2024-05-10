using ContentToolbox.Maps.Generation.Land;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace ContentToolbox.Maps.Generation;

public class MapGenerator
{
    public MapGenerator(LandGenerator landGenerator)
    {
        LandGenerator = landGenerator;
    }

    public LandGenerator LandGenerator { get; }

    public GeneratedMaps Generate()
    {
        Land.Land land = LandGenerator.Generate();
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
