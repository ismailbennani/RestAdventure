using Xtensive.Orm;

namespace RestAdventure.Core.Characters;

[HierarchyRoot]
public class CharacterDbo : Entity
{
    public CharacterDbo(TeamDbo team, string name, CharacterClass characterClass)
    {
        Team = team;
        Name = name;
        Class = characterClass;
    }

    /// <summary>
    ///     The unique ID of the character
    /// </summary>
    [Key]
    [Field]
    public Guid Id { get; private set; }

    [Field]
    public TeamDbo Team { get; private set; }

    /// <summary>
    ///     The name of the character
    /// </summary>
    [Field]
    public string Name { get; private set; }

    /// <summary>
    ///     The class of the character
    /// </summary>
    [Field]
    public CharacterClass Class { get; private set; }
}
