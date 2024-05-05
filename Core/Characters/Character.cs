using RestAdventure.Core.Maps;
using RestAdventure.Kernel;

namespace RestAdventure.Core.Characters;

public record CharacterId(Guid Guid) : Id(Guid);

public class Character : IEquatable<Character>
{
    internal Character(Team team, string name, CharacterClass characterClass, MapLocation location)
    {
        Team = team;
        Name = name;
        Class = characterClass;
        Location = location;
        Inventory = new CharacterInventory(this);
        Jobs = new CharacterJobs(this);
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
    public MapLocation Location { get; private set; }

    /// <summary>
    ///     The inventory of the character
    /// </summary>
    public CharacterInventory Inventory { get; private set; }

    /// <summary>
    ///     The jobs of the character
    /// </summary>
    public CharacterJobs Jobs { get; private set; }

    public void MoveTo(MapLocation location)
    {
        Location = location;
        Team.Player.Discover(location);
    }

    public override string ToString() => $"{Class} {Name} ({Team})";

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
