namespace RestAdventure.Core.Combat.Options;

public class CombatFormationOptions
{
    public static CombatFormationOptions Default { get; } = new();
    public CombatFormationAccessibility Accessibility { get; init; } = CombatFormationAccessibility.Everyone;
}
