namespace RestAdventure.Core.Characters;

public class GameCharacterClasses
{
    readonly Dictionary<CharacterClassId, CharacterClass> _classes = [];

    public IEnumerable<CharacterClass> All => _classes.Values;

    public void Register(CharacterClass item) => _classes[item.Id] = item;
    public CharacterClass? Get(CharacterClassId itemId) => _classes.GetValueOrDefault(itemId);
}

public static class GameCharacterClassExtensions
{
    public static CharacterClass RequireCharacterClass(this GameCharacterClasses classes, CharacterClassId itemId) =>
        classes.Get(itemId) ?? throw new InvalidOperationException($"Could not find item {itemId}");
}
