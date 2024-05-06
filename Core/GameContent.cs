using RestAdventure.Core.Characters;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;
using RestAdventure.Core.Maps;
using RestAdventure.Core.Maps.Harvestables;

namespace RestAdventure.Core;

public class GameContent
{
    /// <summary>
    ///     The characters of the game
    /// </summary>
    public GameCharacters Characters { get; } = new();

    /// <summary>
    ///     The maps of the game
    /// </summary>
    public GameMaps Maps { get; } = new();

    /// <summary>
    ///     The items of the game
    /// </summary>
    public GameItems Items { get; } = new();

    /// <summary>
    ///     The jobs of the game
    /// </summary>
    public GameJobs Jobs { get; } = new();

    /// <summary>
    ///     The harvestables of the game
    /// </summary>
    public GameHarvestables Harvestables { get; } = new();
}
