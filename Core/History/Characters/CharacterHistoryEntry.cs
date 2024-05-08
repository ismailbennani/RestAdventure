using RestAdventure.Core.Entities;
using RestAdventure.Core.History.Entities;
using RestAdventure.Core.Interactions;

namespace RestAdventure.Core.History.Characters;

public abstract class CharacterHistoryEntry : EntityHistoryEntry
{
    protected CharacterHistoryEntry(IInteractingEntity source, long tick) : base(source, tick)
    {
        SourceId = source.Id;
        SourceName = source.Name;
    }

    public GameEntityId SourceId { get; }
    public string SourceName { get; }
}
