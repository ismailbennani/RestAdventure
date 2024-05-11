namespace RestAdventure.Core.Combat;

public class CombatOptions
{
    /// <summary>
    ///     The duration of the preparation phase of the combat
    /// </summary>
    public required int PreparationPhaseDuration { get; init; } = 3;

    /// <summary>
    ///     Arbitrary value for the turn duration. This is used alongside the speed of characters to determine when they play during the combat turn.
    ///     For example, with a combat turn duration of 11 and a speed of 150, the character will play once on the first turn and twice on the second one.
    /// </summary>
    public required int CombatTurnDuration { get; init; } = 100;
}
