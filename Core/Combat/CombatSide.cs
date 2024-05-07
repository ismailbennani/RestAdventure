namespace RestAdventure.Core.Combat;

public enum CombatSide
{
    Team1,
    Team2
}

public static class CombatSideExtensions
{
    public static CombatSide OtherSide(this CombatSide side) =>
        side switch
        {
            CombatSide.Team1 => CombatSide.Team2,
            CombatSide.Team2 => CombatSide.Team1,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
}
