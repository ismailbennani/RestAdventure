namespace RestAdventure.Core.Combat.Old;

public enum CombatSide
{
    Attackers,
    Defenders
}

public static class CombatSideExtensions
{
    public static CombatSide OtherSide(this CombatSide side) =>
        side switch
        {
            CombatSide.Attackers => CombatSide.Defenders,
            CombatSide.Defenders => CombatSide.Attackers,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
}
