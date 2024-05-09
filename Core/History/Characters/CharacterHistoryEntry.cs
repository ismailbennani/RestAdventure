using RestAdventure.Core.Entities;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Core.History.Characters;

public abstract class CharacterHistoryEntry : EntityHistoryEntry
{
    protected CharacterHistoryEntry(Character character, long tick) : base(character, tick)
    {
        SourceId = character.Id;
        SourceName = character.Name;
    }

    public GameEntityId SourceId { get; }
    public string SourceName { get; }
}
