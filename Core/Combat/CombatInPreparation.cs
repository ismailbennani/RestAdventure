using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core.Combat;

public class CombatInPreparation
{
    public CombatInPreparation(IGameEntityWithCombatStatistics attacker, IGameEntityWithCombatStatistics target, GameSettings settings)
    {
        Location = attacker.Location;
        Attackers = new CombatFormationInPreparation(attacker, settings);
        Defenders = new CombatFormationInPreparation(target, settings);
        Settings = settings;
    }

    public CombatInstanceId Id { get; } = new(Guid.NewGuid());
    public Location Location { get; }
    public CombatFormationInPreparation Attackers { get; }
    public CombatFormationInPreparation Defenders { get; }
    public GameSettings Settings { get; }
    public int Turn { get; private set; }
    public bool Canceled { get; private set; }

    public void Tick() => Turn++;
    public void Cancel() => Canceled = true;

    public CombatFormationInPreparation GetTeam(CombatSide side) =>
        side switch
        {
            CombatSide.Attackers => Attackers,
            CombatSide.Defenders => Defenders,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null)
        };
}
