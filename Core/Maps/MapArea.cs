namespace RestAdventure.Core.Maps;

public class MapArea : IEquatable<MapArea>
{
    internal MapArea(GameMaps gameMaps, string name)
    {
        GameMaps = gameMaps;
        Name = name;
    }

    internal GameMaps GameMaps { get; }

    public MapAreaId Id { get; } = new(Guid.NewGuid());
    public string Name { get; set; }
    public IEnumerable<MapLocation> Locations => GameMaps.GetAreaLocations(this);

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
