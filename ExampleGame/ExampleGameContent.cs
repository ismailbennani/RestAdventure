using ExampleGame.Characters;
using ExampleGame.Jobs;
using ExampleGame.Maps;
using ExampleGame.Monsters;
using RestAdventure.Core.Content;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Plugins;

namespace ExampleGame;

public class ExampleGameContent : ContentPlugin
{
    public MapGenerator MapGenerator { get; }
    public GeneratedMaps GeneratedMaps { get; }
    public CharacterClasses CharacterClasses { get; }
    public Rattlings Rattlings { get; }
    public Gatherer Gatherer { get; }

    public ExampleGameContent()
    {
        MapGenerator = new MapGenerator();
        GeneratedMaps = MapGenerator.GenerateMaps();
        CharacterClasses = new CharacterClasses(GeneratedMaps);
        Rattlings = new Rattlings();
        Gatherer = new Gatherer();
    }

    public override Task AddContentAsync(GameContent content)
    {
        RegisterContent(content, CharacterClasses);
        RegisterContent(content, Rattlings);
        RegisterContent(content, Gatherer);

        content.Maps.Areas.Register(GeneratedMaps.Areas);
        content.Maps.Locations.Register(GeneratedMaps.Locations);
        content.Maps.Locations.Connect(GeneratedMaps.Connections);

        return Task.CompletedTask;
    }

    void RegisterJobs(GameContent content)
    {
        content.StaticObjects.Register(ObjectExplorer.FindValuesOfType<Gatherer, StaticObject>(Gatherer));
        content.Items.Register(ObjectExplorer.FindValuesOfType<Gatherer, Item>(Gatherer));
        content.Jobs.Register(Gatherer.Job);
    }

    void RegisterContent<T>(GameContent content, T instance)
    {
        content.Characters.Classes.Register(ObjectExplorer.FindValuesOfType<T, CharacterClass>(instance));
        content.StaticObjects.Register(ObjectExplorer.FindValuesOfType<T, StaticObject>(instance));
        content.Items.Register(ObjectExplorer.FindValuesOfType<T, Item>(instance));
        content.Jobs.Register(ObjectExplorer.FindValuesOfType<T, Job>(instance));
        content.Maps.Areas.Register(ObjectExplorer.FindValuesOfType<T, MapArea>(instance));
        content.Maps.Locations.Register(ObjectExplorer.FindValuesOfType<T, Location>(instance));
        content.Monsters.Species.Register(ObjectExplorer.FindValuesOfType<T, MonsterSpecies>(instance));
        content.Monsters.Families.Register(ObjectExplorer.FindValuesOfType<T, MonsterFamily>(instance));
    }
}
