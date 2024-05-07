using BaseGame.Jobs;
using BaseGame.Maps;
using BaseGame.Monsters;
using BaseGame.StaticObjects;
using RestAdventure.Core;
using RestAdventure.Core.Plugins;

namespace BaseGame;

public class BaseGameContent : ContentPlugin
{
    public MapGenerator MapGenerator { get; }
    public GeneratedMaps GeneratedMaps { get; }
    public CharacterClasses CharacterClasses { get; }
    public Items Items { get; }
    public Rattlings Rattlings { get; }
    public Trees Trees { get; }
    public Gatherer Gatherer { get; }

    public BaseGameContent()
    {
        MapGenerator = new MapGenerator();
        GeneratedMaps = MapGenerator.GenerateMaps();
        CharacterClasses = new CharacterClasses(GeneratedMaps);
        Items = new Items();
        Rattlings = new Rattlings();
        Trees = new Trees();
        Gatherer = new Gatherer(Trees, Items);
    }

    public override Task AddContentAsync(GameContent content)
    {
        content.Monsters.Families.Register(Rattlings.Family);
        content.Monsters.Species.Register(Rattlings.Species);

        content.StaticObjects.Register(Trees.AppleTree);
        content.StaticObjects.Register(Trees.PearTree);

        content.Jobs.Register(Gatherer.Job);

        content.Characters.Classes.Register(CharacterClasses.All);
        content.Maps.Areas.Register(GeneratedMaps.Areas);
        content.Maps.Locations.Register(GeneratedMaps.Locations);
        content.Maps.Locations.Connect(GeneratedMaps.Connections);
        content.Items.Register(Items.All);
        content.Jobs.Register(Gatherer.All);

        return Task.CompletedTask;
    }
}
