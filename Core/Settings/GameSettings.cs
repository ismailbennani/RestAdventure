namespace RestAdventure.Core.Settings;

public class GameSettings
{
    /// <summary>
    ///     The max number of characters in a team
    /// </summary>
    public int MaxTeamSize { get; set; } = 3;

    public CombatSettings Combat { get; } = new();
}

public class CombatSettings
{
    /// <summary>
    ///     Arbitrary value for the turn duration. This is used alongside the speed of characters to determine when they play during the combat turn.
    ///     For example, with a combat turn duration of 11 and a speed of 150, the character will play once on the first turn and twice on the second one.
    /// </summary>
    public int CombatTurnDuration { get; } = 100;
}
