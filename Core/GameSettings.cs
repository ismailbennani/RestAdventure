namespace RestAdventure.Core;

public class GameSettings
{
    public bool GenerateEntitiesOnInitialization = true;

    /// <summary>
    ///     The max number of characters in a team
    /// </summary>
    public int MaxTeamSize { get; set; } = 3;

    /// <summary>
    ///     Combat settings
    /// </summary>
    public CombatSettings Combat { get; } = new();
}

public class CombatSettings
{
    public int MaxCharacterTeamSize = 3;
    public int MaxMonsterTeamSize = 8;

    /// <summary>
    ///     The duration of the preparation phase of the combat
    /// </summary>
    public int PreparationPhaseDuration { get; } = 3;

    /// <summary>
    ///     Arbitrary value for the turn duration. This is used alongside the speed of characters to determine when they play during the combat turn.
    ///     For example, with a combat turn duration of 11 and a speed of 150, the character will play once on the first turn and twice on the second one.
    /// </summary>
    public int CombatTurnDuration { get; } = 100;
}
