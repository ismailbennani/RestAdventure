using BaseGame.Maps;
using BaseGame.Monsters;
using BaseGame.StaticObjects;
using RestAdventure.Core;
using RestAdventure.Core.Characters;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Plugins;

namespace BaseGame;

public class BaseGameContent : ContentPlugin
{
    public override Task AddContentAsync(GameContent content)
    {
        MapGenerator mapGenerator = new();
        GeneratedMaps generatedMaps = mapGenerator.GenerateMaps();
        CharacterClasses characterClasses = new(generatedMaps);
        Items items = new();
        Jobs jobs = new();

        Rattlings rattlings = new();
        content.Monsters.Families.Register(rattlings.Family);
        content.Monsters.Species.Register(rattlings.Species);

        Trees trees = new();
        content.StaticObjects.Register(trees.AppleTree);
        content.StaticObjects.Register(trees.PearTree);

        RegisterMaps(content, generatedMaps);
        RegisterCharacterClasses(content, characterClasses.All);
        RegisterItems(content, items.All);
        RegisterJobs(content, jobs.All);

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
    }
}
