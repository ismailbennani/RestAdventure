using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;

namespace RestAdventure.Core.History.Combats;

public abstract class CombatHistoryEntry : HistoryEntry
{
    public CombatHistoryEntry(CombatInstance combatInstance, long tick) : base(tick)
    {
        CombatInstanceId = combatInstance.Id;
        Turn = combatInstance.Turn;
        Team1 = combatInstance.Team1.Entities.Select(e => (e.Id, e.Name)).ToArray();
        Team2 = combatInstance.Team2.Entities.Select(e => (e.Id, e.Name)).ToArray();
    }

    public CombatInstanceId CombatInstanceId { get; }
    public int Turn { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Team1 { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Team2 { get; }
}
