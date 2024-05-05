using RestAdventure.Core.Players;
using RestAdventure.Kernel;

namespace RestAdventure.Core.Characters;

public record TeamId(Guid Guid) : Id(Guid);

public class Team : IEquatable<Team>
{
    internal Team(GameCharactersState gameCharactersState, PlayerId playerId)
    {
        GameCharactersState = gameCharactersState;
        PlayerId = playerId;
    }

    internal GameCharactersState GameCharactersState { get; }

    public TeamId Id { get; } = new(Guid.NewGuid());
    public PlayerId PlayerId { get; private set; }
    public IEnumerable<Character> Characters => GameCharactersState.GetCharactersInTeam(this);

    public bool Equals(Team? other)
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
        return Equals((Team)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Team? left, Team? right) => Equals(left, right);

    public static bool operator !=(Team? left, Team? right) => !Equals(left, right);
}
