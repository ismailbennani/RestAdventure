namespace RestAdventure.Game.Settings;

/// <summary>
///     Server settings
/// </summary>
public class ServerSettings
{
    /// <summary>
    ///     The time to wait between ticks.
    /// </summary>
    public TimeSpan TickDuration { get; init; } = TimeSpan.FromSeconds(30);
}
