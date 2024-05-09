using RestAdventure.Core.Content;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Spawners;

namespace RestAdventure.Core.Plugins;

public class Scenario
{
    public string? Name { get; init; }

    public List<MapArea> Areas { get; } = new();
    public List<Location> Locations { get; } = new();
    public List<(Location, Location)> Connections { get; } = new();
    public List<Spawner> Spawners { get; } = new();

    public List<StaticObject> StaticObjects { get; } = new();

    public List<MonsterFamily> MonsterFamilies { get; } = new();
    public List<MonsterSpecies> MonsterSpecies { get; } = new();

    public List<CharacterClass> CharacterClasses { get; } = new();

    public List<Item> Items { get; } = new();

    public List<Job> Jobs { get; } = new();

    internal GameContent ToGameContent()
    {
        GameContent content = new() { Title = Name ?? "Game" };

        content.Maps.Areas.Register(Areas);
        content.Maps.Locations.Register(Locations);
        content.Maps.Locations.Register(Connections);
        content.Maps.Spawners.Register(Spawners);

        content.StaticObjects.Register(StaticObjects);

        content.Monsters.Families.Register(MonsterFamilies);
        content.Monsters.Species.Register(MonsterSpecies);

        content.Characters.Classes.Register(CharacterClasses);

        content.Items.Register(Items);

        content.Jobs.Register(Jobs);

        return content;
    }
}
