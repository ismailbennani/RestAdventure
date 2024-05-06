using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Harvestables;
using RestAdventure.Core.Maps.Locations;

namespace BaseGame.Maps;

public class MapGenerator
{
    readonly Harvestables _harvestables;

    public MapGenerator(Harvestables harvestables)
    {
        _harvestables = harvestables;
    }

    public GeneratedMaps GenerateMaps()
    {
        MapArea area = new() { Name = "Start" };
        Location location1 = new() { Area = area, PositionX = 0, PositionY = 0 };
        Location location2 = new() { Area = area, PositionX = 0, PositionY = 1 };
        HarvestableInstance appleTreeInstance = new(_harvestables.AppleTree, location1);
        HarvestableInstance pearTreeInstance = new(_harvestables.PearTree, location1);

        return new GeneratedMaps
        {
            Areas = [area],
            Locations = [location1, location2],
            Connections = [(location1, location2)],
            Harvestables = [appleTreeInstance, pearTreeInstance]
        };
    }
}
