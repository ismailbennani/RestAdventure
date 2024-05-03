using RestAdventure.Core.Maps;
using Xtensive.Orm;

namespace RestAdventure.Core.Characters;

[HierarchyRoot]
public class CharacterDbo : Entity
{
    public CharacterDbo(TeamDbo team, string name, CharacterClass characterClass) : this(team, name, characterClass, Query.All<MapLocationDbo>().First()) { }

    public CharacterDbo(TeamDbo team, string name, CharacterClass characterClass, MapLocationDbo location)
    {
        Team = team;
        Name = name;
        Class = characterClass;
        Location = location;
    }

    /// <summary>
    ///     The unique ID of the character
    /// </summary>
    [Key]
    [Field]
    public Guid Id { get; private set; }

    /// <summary>
    ///     The team of the character
    /// </summary>
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

    /// <summary>
    ///     The location of the character
    /// </summary>
    [Field]
    public MapLocationDbo Location { get; private set; }
}
