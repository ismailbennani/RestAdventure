using RestAdventure.Core.Items;
using RestAdventure.Core.Maps;

namespace RestAdventure.Core;

public class GameContent
{
    /// <summary>
    ///     The maps of the game
    /// </summary>
    public GameMaps Maps { get; } = new();

    /// <summary>
    ///     The items of the game
    /// </summary>
    public GameItems Items { get; } = new();
}
