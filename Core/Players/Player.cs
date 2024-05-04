namespace RestAdventure.Core.Players;

public class Player : IEquatable<Player>
{
    public Player(Guid id, string name)
    {
        Id = id;
        Name = name;
        ApiKey = Guid.NewGuid();
    }

    public Guid Id { get; }
    public string Name { get; set; }
    public Guid ApiKey { get; private set; }

    public Guid RefreshApiKey() => ApiKey = Guid.NewGuid();

    public bool Equals(Player? other)
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
        return Equals((Player)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Player? left, Player? right) => Equals(left, right);

    public static bool operator !=(Player? left, Player? right) => !Equals(left, right);
}
