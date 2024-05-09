using RestAdventure.Core.Content;

namespace RestAdventure.Core.Plugins;

/// <summary>
///     A plugin that adds content to the game
/// </summary>
public abstract class ContentPlugin
{
    /// <summary>
    ///     Add content to the game.
    /// </summary>
    public abstract Task AddContentAsync(GameContent content);
}
