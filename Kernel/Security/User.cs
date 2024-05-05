namespace RestAdventure.Kernel.Security;

public record UserId(Guid Guid) : Id(Guid);

public record ApiKey(Guid Guid);

public class User : IEquatable<User>
{
    public User(UserId id, string name)
    {
        Id = id;
        Name = name;
        ApiKey = new ApiKey(Guid.NewGuid());
    }

    public UserId Id { get; }
    public string Name { get; set; }
    public ApiKey ApiKey { get; private set; }

    public ApiKey RefreshApiKey() => ApiKey = new ApiKey(Guid.NewGuid());

    public bool Equals(User? other)
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
        return Equals((User)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(User? left, User? right) => Equals(left, right);

    public static bool operator !=(User? left, User? right) => !Equals(left, right);
}
