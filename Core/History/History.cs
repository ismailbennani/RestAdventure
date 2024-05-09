using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Core.History;

public class GameHistory
{
    readonly List<HistoryEntry> _entries = [];

    public IEnumerable<HistoryEntry> All => _entries;

    public void Record(HistoryEntry entry) => _entries.Add(entry);

    public IEnumerable<EntityHistoryEntry> Character(Character character) => _entries.OfType<EntityHistoryEntry>().Where(c => c.EntityId == character.Id);
}
