using RestAdventure.Core.Characters;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Monsters;
using RestAdventure.Core.StaticObjects;

namespace RestAdventure.Core.Content;

public class GameContent
{
    /// <summary>
    ///     The maps of the game
    /// </summary>
    public GameMaps Maps { get; } = new();

    /// <summary>
    ///     The static objects of the game
    /// </summary>
    public GameStaticObjects StaticObjects { get; } = new();

    /// <summary>
    ///     The items of the game
    /// </summary>
    public GameItems Items { get; } = new();

    /// <summary>
    ///     The jobs of the game
    /// </summary>
    public GameJobs Jobs { get; } = new();

    /// <summary>
    ///     The characters of the game
    /// </summary>
    public GameCharacters Characters { get; } = new();

    /// <summary>
    ///     The mosnters of the game
    /// </summary>
    public GameMonsters Monsters { get; } = new();
}
