using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Harvestables;
using RestAdventure.Core.Maps.Locations;

namespace BaseGame.Maps;

public class MapGenerator
{
    public GeneratedMaps GenerateMaps()
    {
        MapArea area = new() { Name = "Start" };
        Location location1 = new() { Area = area, PositionX = 0, PositionY = 0 };
        Location location2 = new() { Area = area, PositionX = 0, PositionY = 1 };

        return new GeneratedMaps
        {
            Areas = [area],
            Locations = [location1, location2],
            Connections = [(location1, location2)]
        };
    }

    public IEnumerable<HarvestableInstance> GenerateHarvestables(GeneratedMaps maps, Harvestables harvestables)
    {
        HarvestableInstance appleTreeInstance = new(harvestables.AppleTree, maps.Locations.First());
        HarvestableInstance pearTreeInstance = new(harvestables.PearTree, maps.Locations.First());
        return [appleTreeInstance, pearTreeInstance];
    }
}
