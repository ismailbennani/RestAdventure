namespace RestAdventure.Core.Maps;

public class MapArea : IEquatable<MapArea>
{
    internal MapArea(GameMapState gameMapState, string name)
    {
        GameMapState = gameMapState;
        Name = name;
    }

    internal GameMapState GameMapState { get; }

    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public IEnumerable<MapLocation> Locations => GameMapState.GetAreaLocations(this);

    public override string ToString() => $"{Name} ({Id})";

    public bool Equals(MapArea? other)
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
        return Equals((MapArea)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(MapArea? left, MapArea? right) => Equals(left, right);

    public static bool operator !=(MapArea? left, MapArea? right) => !Equals(left, right);
}
