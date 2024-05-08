using RestAdventure.Core.Maps.Locations;
using RestAdventure.Core.Settings;

namespace RestAdventure.Core.Combat;

public class CombatInPreparation
{
    internal CombatInPreparation(IReadOnlyList<IGameEntityWithCombatStatistics> attackers, IReadOnlyList<IGameEntityWithCombatStatistics> defenders, GameSettings settings)
    {
        Location = attackers[0].Location;
        Attackers = new CombatFormationInPreparation(attackers, settings);
        Defenders = new CombatFormationInPreparation(defenders, settings);
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
