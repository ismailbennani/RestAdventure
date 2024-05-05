using RestAdventure.Core.Maps.Areas;
using RestAdventure.Kernel;

namespace RestAdventure.Core.Maps.Locations;

public record MapLocationId(Guid Guid) : Id(Guid);

public class MapLocation : IEquatable<MapLocation>
{
    public MapLocationId Id { get; } = new(Guid.NewGuid());
    public required MapArea Area { get; init; }
    public required int PositionX { get; init; }
    public required int PositionY { get; init; }

    public override string ToString() => $"{Area}[{PositionX}, {PositionY}] ({Id})";

    public bool Equals(MapLocation? other)
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
        return Equals((MapLocation)obj);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(MapLocation? left, MapLocation? right) => Equals(left, right);

    public static bool operator !=(MapLocation? left, MapLocation? right) => !Equals(left, right);
}
