using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core.Combat;

public class CombatInPreparation
{
    public CombatInPreparation(IGameEntityWithCombatStatistics attacker, IGameEntityWithCombatStatistics target, GameSettings settings)
    {
        Location = attacker.Location;
        Team1 = new CombatFormationInPreparation(attacker, settings);
        Team2 = new CombatFormationInPreparation(target, settings);
        Settings = settings;
    }

    public CombatInstanceId Id { get; } = new(Guid.NewGuid());
    public Location Location { get; }
    public CombatFormationInPreparation Team1 { get; }
    public CombatFormationInPreparation Team2 { get; }
    public GameSettings Settings { get; }
    public int Turn { get; private set; }
    public bool Canceled { get; private set; }

    public void Tick() => Turn++;
    public void Cancel() => Canceled = true;
}
