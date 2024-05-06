using RestAdventure.Core.Characters;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Core.History.Characters;

public abstract class CharacterHistoryEntry : EntityHistoryEntry
{
    protected CharacterHistoryEntry(Character entity, long tick) : base(entity, tick)
    {
        CharacterId = entity.Id;
        CharacterName = entity.Name;
    }

    public CharacterId CharacterId { get; }
    public string CharacterName { get; }
}
