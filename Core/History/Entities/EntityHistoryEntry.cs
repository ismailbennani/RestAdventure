using RestAdventure.Core.Entities;

namespace RestAdventure.Core.History.Entities;

public abstract class EntityHistoryEntry : HistoryEntry
{
    protected EntityHistoryEntry(IGameEntity entity, long tick) : base(tick)
    {
        EntityId = entity.Id;
        EntityName = entity.Name;
    }

    public GameEntityId EntityId { get; }
    public string EntityName { get; }
}
