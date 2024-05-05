using RestAdventure.Core.Maps;

namespace RestAdventure.Core;

public class GameContent
{
    public GameContent()
    {
        Maps = new GameMaps(this);
    }

    /// <summary>
    ///     The maps of the game
    /// </summary>
    public GameMaps Maps { get; }
}
