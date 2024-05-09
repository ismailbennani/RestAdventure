using RestAdventure.Core.Combat;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.History.Combats;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Core.History;

public class GameHistory
{
    readonly List<HistoryEntry> _entries = [];

    public IEnumerable<HistoryEntry> All => _entries;

    public void Record(HistoryEntry entry) => _entries.Add(entry);

    public IEnumerable<EntityHistoryEntry> Character(CharacterId characterId) => _entries.OfType<EntityHistoryEntry>().Where(c => c.EntityId == characterId);

    public IEnumerable<CombatHistoryEntry> Combat(CombatInstanceId combatId) => _entries.OfType<CombatHistoryEntry>().Where(c => c.CombatInstanceId == combatId);

    public IEnumerable<CombatHistoryEntry> Combat(GameEntityId entityId) =>
        _entries.OfType<CombatHistoryEntry>().Where(c => c.Attackers.Any(a => a.Id == entityId) || c.Defenders.Any(a => a.Id == entityId));
}
