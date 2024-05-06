using RestAdventure.Core.Characters;
using RestAdventure.Core.History.Entities;

namespace RestAdventure.Core.History.Characters;

public abstract class CharacterHistoryEntry : EntityHistoryEntry
{
    protected CharacterHistoryEntry(Character character, long tick) : base(character, tick)
    {
        CharacterId = character.Id;
        CharacterName = character.Name;
    }

    public CharacterId CharacterId { get; }
    public string CharacterName { get; }
}
