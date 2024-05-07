namespace RestAdventure.Core.Combat;

public class CombatFormation
{
    /// <summary>
    ///     The characters in order
    /// </summary>
    public required IReadOnlyList<IGameEntityWithCombatStatistics> Entities { get; init; }
}
