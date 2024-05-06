using BaseGame.Maps;
using RestAdventure.Core;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Harvestables;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Plugins;

namespace BaseGame;

public class BaseGameContent : ContentPlugin
{
    public override Task AddContentAsync(GameContent content)
    {
        CharacterClasses characterClasses = new();
        content.Characters.Classes.Register(characterClasses.Knight);
        content.Characters.Classes.Register(characterClasses.Mage);
        content.Characters.Classes.Register(characterClasses.Scout);
        content.Characters.Classes.Register(characterClasses.Dealer);

        Items items = new();
        content.Items.Register(items.Apple);
        content.Items.Register(items.Pear);

        Jobs jobs = new();
        content.Jobs.Register(jobs.Gatherer);

        Harvestables harvestables = new(items, jobs);
        content.Harvestables.Register(harvestables.AppleTree);
        content.Harvestables.Register(harvestables.PearTree);

        GeneratedMaps generatedMaps = new MapGenerator(harvestables).GenerateMaps();
        RegisterMaps(content, generatedMaps);

        return Task.CompletedTask;
    }

    static void RegisterMaps(GameContent content, GeneratedMaps generatedMaps)
    {
        foreach (MapArea area in generatedMaps.Areas)
        {
            content.Maps.Areas.Register(area);
        }

        foreach (Location location in generatedMaps.Locations)
        {
            content.Maps.Locations.Register(location);
        }

        foreach ((Location location1, Location location2) in generatedMaps.Connections)
        {
            content.Maps.Locations.Connect(location1, location2);
        }

        foreach (HarvestableInstance harvestableInstance in generatedMaps.Harvestables)
        {
            content.Maps.Harvestables.Register(harvestableInstance);
        }
    }
}
