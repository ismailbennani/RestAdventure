using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.History.Combats;

public abstract class CombatHistoryEntry : HistoryEntry
{
    public CombatHistoryEntry(CombatInstance combatInstance, long tick) : base(tick)
    {
        CombatInstanceId = combatInstance.Id;
        LocationId = combatInstance.Location.Id;
        LocationAreaId = combatInstance.Location.Area.Id;
        LocationAreaName = combatInstance.Location.Area.Name;
        LocationPositionX = combatInstance.Location.PositionX;
        LocationPositionY = combatInstance.Location.PositionY;
        Attackers = combatInstance.Attackers.Entities.Select(e => (e.Id, e.Name)).ToArray();
        Defenders = combatInstance.Defenders.Entities.Select(e => (e.Id, e.Name)).ToArray();
    }

    public CombatInstanceId CombatInstanceId { get; }
    public LocationId LocationId { get; }
    public MapAreaId LocationAreaId { get; }
    public string LocationAreaName { get; }
    public int LocationPositionX { get; }
    public int LocationPositionY { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Attackers { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Defenders { get; }
}
