using ExampleGame.Characters;
using ExampleGame.Jobs;
using ExampleGame.Maps;
using ExampleGame.Monsters;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Plugins;
using RestAdventure.Core.Spawners;

namespace ExampleGame;

public class ExampleGameScenarioBuilder
{
    public MapGenerator MapGenerator { get; }
    public GeneratedMaps GeneratedMaps { get; }
    public CharacterClasses CharacterClasses { get; }
    public Rattlings Rattlings { get; }
    public Gatherer Gatherer { get; }

    public ExampleGameScenarioBuilder()
    {
        MapGenerator = new MapGenerator();
        GeneratedMaps = MapGenerator.GenerateMaps();
        CharacterClasses = new CharacterClasses(GeneratedMaps);
        Rattlings = new Rattlings();
        Gatherer = new Gatherer();
    }

    public Scenario Build()
    {
        Scenario scenario = new() { Name = "Rat Attack" };

        ExtractContent(scenario, CharacterClasses);
        ExtractContent(scenario, Rattlings);
        ExtractContent(scenario, Gatherer);
        ExtractContent(scenario, GeneratedMaps);

        scenario.Spawners.Add(new StaticObjectRandomAreaSpawner(Gatherer.AppleTree, GeneratedMaps.Areas.First(), 5));

        return scenario;
    }

    static void ExtractContent<T>(Scenario scenario, T instance)
    {
        scenario.CharacterClasses.AddRange(ObjectExplorer.FindValuesOfType<T, CharacterClass>(instance).ToArray());
        scenario.StaticObjects.AddRange(ObjectExplorer.FindValuesOfType<T, StaticObject>(instance).ToArray());
        scenario.Items.AddRange(ObjectExplorer.FindValuesOfType<T, Item>(instance).ToArray());
        scenario.Jobs.AddRange(ObjectExplorer.FindValuesOfType<T, Job>(instance).ToArray());
        scenario.Areas.AddRange(ObjectExplorer.FindValuesOfType<T, MapArea>(instance).ToArray());
        scenario.Locations.AddRange(ObjectExplorer.FindValuesOfType<T, Location>(instance).ToArray());
        scenario.Connections.AddRange(ObjectExplorer.FindValuesOfType<T, (Location, Location)>(instance).ToArray());
        scenario.Spawners.AddRange(ObjectExplorer.FindValuesOfType<T, Spawner>(instance).ToArray());
        scenario.MonsterFamilies.AddRange(ObjectExplorer.FindValuesOfType<T, MonsterFamily>(instance).ToArray());
        scenario.MonsterSpecies.AddRange(ObjectExplorer.FindValuesOfType<T, MonsterSpecies>(instance).ToArray());
    }
}
