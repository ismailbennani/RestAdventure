using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.History.Combats;

public abstract class CombatHistoryEntry : HistoryEntry
{
    public CombatHistoryEntry(CombatInPreparation combat, long tick) : base(tick)
    {
        CombatInstanceId = combat.Id;
        LocationId = combat.Location.Id;
        LocationAreaId = combat.Location.Area.Id;
        LocationAreaName = combat.Location.Area.Name;
        LocationPositionX = combat.Location.PositionX;
        LocationPositionY = combat.Location.PositionY;
        Attackers = combat.Attackers.Entities.Select(e => (e.Id, e.Name)).ToArray();
        Defenders = combat.Defenders.Entities.Select(e => (e.Id, e.Name)).ToArray();
        Turn = combat.Turn;
    }

    public CombatHistoryEntry(CombatInstance combat, long tick) : base(tick)
    {
        CombatInstanceId = combat.Id;
        LocationId = combat.Location.Id;
        LocationAreaId = combat.Location.Area.Id;
        LocationAreaName = combat.Location.Area.Name;
        LocationPositionX = combat.Location.PositionX;
        LocationPositionY = combat.Location.PositionY;
        Attackers = combat.Attackers.Entities.Select(e => (e.Id, e.Name)).ToArray();
        Defenders = combat.Defenders.Entities.Select(e => (e.Id, e.Name)).ToArray();
    }

    public CombatInstanceId CombatInstanceId { get; }
    public LocationId LocationId { get; }
    public MapAreaId LocationAreaId { get; }
    public string LocationAreaName { get; }
    public int LocationPositionX { get; }
    public int LocationPositionY { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Attackers { get; }
    public IReadOnlyList<(GameEntityId Id, string Name)> Defenders { get; }
    public int Turn { get; }
}
