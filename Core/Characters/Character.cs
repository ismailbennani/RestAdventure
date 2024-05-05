using RestAdventure.Core.Characters.Notifications;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Maps.Locations;

namespace RestAdventure.Core.Characters;

public record CharacterId(Guid Guid) : EntityId(Guid);

public class Character : Entity<CharacterId>
{
    internal Character(Team team, string name, CharacterClass characterClass, Location location) : base(new CharacterId(Guid.NewGuid()), name, location)
    {
        Team = team;
        Class = characterClass;
        Inventory = new CharacterInventory(this);
        Jobs = new CharacterJobs(this);
    }

    /// <summary>
    ///     The team of the character
    /// </summary>
    public Team Team { get; private set; }

    /// <summary>
    ///     The class of the character
    /// </summary>
    public CharacterClass Class { get; private set; }

    /// <summary>
    ///     The inventory of the character
    /// </summary>
    public CharacterInventory Inventory { get; private set; }

    /// <summary>
    ///     The jobs of the character
    /// </summary>
    public CharacterJobs Jobs { get; private set; }

    public async Task MoveToAsync(Location location)
    {
        if (Location == location)
        {
            return;
        }

        Location = location;
        await Team.GameCharacters.GameState.Publisher.Publish(new CharacterMovedToLocation { Character = this, Location = location });
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
