using RestAdventure.Core.Maps;

namespace RestAdventure.Core.Characters;

public class Character : IEquatable<Character>
{
    internal Character(Team team, string name, CharacterClass characterClass, MapLocation location)
    {
        Team = team;
        Name = name;
        Class = characterClass;
        Location = location;
    }

    /// <summary>
    ///     The unique ID of the character
    /// </summary>
    public CharacterId Id { get; } = new(Guid.NewGuid());

    /// <summary>
    ///     The team of the character
    /// </summary>
    public Team Team { get; private set; }

    /// <summary>
    ///     The name of the character
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     The class of the character
    /// </summary>
    public CharacterClass Class { get; private set; }

    /// <summary>
    ///     The location of the character
    /// </summary>
    public MapLocation Location { get; set; }

    public bool Equals(Character? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        if (obj.GetType() != GetType())
        {
            return false;
        }
        return Equals((Character)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Character? left, Character? right) => Equals(left, right);

    public static bool operator !=(Character? left, Character? right) => !Equals(left, right);
}
