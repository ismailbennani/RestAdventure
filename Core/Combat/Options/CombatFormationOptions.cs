namespace RestAdventure.Core.Combat.Options;

public class CombatFormationOptions
{
    public required CombatFormationAccessibility Accessibility { get; init; }
    public required int MaxCount { get; init; }

    public static CombatFormationOptions DefaultCharacterTeamOptions(GameState state) =>
        new()
        {
            Accessibility = CombatFormationAccessibility.Everyone,
            MaxCount = state.Settings.Combat.MaxCharacterTeamSize
        };

    public static CombatFormationOptions DefaultMonsterTeamOptions(GameState state) =>
        new()
        {
            Accessibility = CombatFormationAccessibility.Everyone,
            MaxCount = state.Settings.Combat.MaxMonsterTeamSize
        };
}
