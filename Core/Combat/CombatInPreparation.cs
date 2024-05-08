using RestAdventure.Core.Settings;

namespace RestAdventure.Core.Combat;

public class CombatInPreparation
{
    public CombatInPreparation(IGameEntityWithCombatStatistics attacker, IGameEntityWithCombatStatistics target, GameSettings settings)
    {
        Team1 = new CombatFormationInPreparation(attacker, settings);
        Team2 = new CombatFormationInPreparation(target, settings);
        Settings = settings;
    }

    public CombatFormationInPreparation Team1 { get; }
    public CombatFormationInPreparation Team2 { get; }
    public GameSettings Settings { get; }
}
