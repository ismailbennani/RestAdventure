using BaseGame.Maps;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
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
        RegisterCharacterClasses(content, characterClasses.All);

        Items items = new();
        RegisterItems(content, items.All);

        Jobs jobs = new();
        RegisterJobs(content, jobs.All);

        Harvestables harvestables = new(items, jobs);
        RegisterHarvestables(content, harvestables.All);

        GeneratedMaps generatedMaps = new MapGenerator(harvestables).GenerateMaps();
        RegisterMaps(content, generatedMaps);

        return Task.CompletedTask;
    }

    static void RegisterCharacterClasses(GameContent content, IEnumerable<CharacterClass> classes)
    {
        foreach (CharacterClass cls in classes)
        {
            content.Characters.Classes.Register(cls);
        }
    }

    static void RegisterItems(GameContent content, IEnumerable<Item> items)
    {
        foreach (Item item in items)
        {
            content.Items.Register(item);
        }
    }

    static void RegisterJobs(GameContent content, IEnumerable<Job> jobs)
    {
        foreach (Job job in jobs)
        {
            content.Jobs.Register(job);
        }
    }

    static void RegisterHarvestables(GameContent content, IEnumerable<Harvestable> harvestables)
    {
        foreach (Harvestable harvestable in harvestables)
        {
            content.Harvestables.Register(harvestable);
        }
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
