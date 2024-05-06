using RestAdventure.Core.Entities;

namespace RestAdventure.Core.History.Entities;

public abstract class EntityHistoryEntry : HistoryEntry
{
    protected EntityHistoryEntry(IGameEntity character, long tick) : base(tick)
    {
        EntityId = character.Id;
        EntityName = character.Name;
    }

    public GameEntityId EntityId { get; }
    public string EntityName { get; }
}
