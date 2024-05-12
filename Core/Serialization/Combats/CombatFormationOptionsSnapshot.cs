using RestAdventure.Core.Combat.Options;

namespace RestAdventure.Core.Serialization.Combats;

public class CombatFormationOptionsSnapshot
{
    public required CombatFormationAccessibility Accessibility { get; init; }
    public required int MaxCount { get; init; }

    public static CombatFormationOptionsSnapshot Take(CombatFormationOptions options) =>
        new()
        {
            Accessibility = options.Accessibility,
            MaxCount = options.MaxCount
        };
}
