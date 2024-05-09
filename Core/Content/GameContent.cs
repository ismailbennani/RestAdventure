using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.Monsters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Content;

public class GameContent
{
    public string Title { get; init; } = "Game";

    /// <summary>
    ///     The maps of the game
    /// </summary>
    public GameMaps Maps { get; } = new();

    /// <summary>
    ///     The static objects of the game
    /// </summary>
    public GameResourcesStore<StaticObjectId, StaticObject> StaticObjects { get; } = new();

    /// <summary>
    ///     The items of the game
    /// </summary>
    public GameResourcesStore<ItemId, Item> Items { get; } = new();

    /// <summary>
    ///     The jobs of the game
    /// </summary>
    public GameJobs Jobs { get; } = new();

    /// <summary>
    ///     The characters of the game
    /// </summary>
    public GameCharacters Characters { get; } = new();

    /// <summary>
    ///     The monsters of the game
    /// </summary>
    public GameMonsters Monsters { get; } = new();
}
