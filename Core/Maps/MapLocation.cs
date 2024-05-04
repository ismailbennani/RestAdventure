﻿using RestAdventure.Core.Characters;

namespace RestAdventure.Core.Maps;

public class MapLocation : IEquatable<MapLocation>
{
    internal MapLocation(MapArea area, int x, int y)
    {
        Area = area;
        PositionX = x;
        PositionY = y;
    }

    public Guid Id { get; } = Guid.NewGuid();
    public MapArea Area { get; }
    public int PositionX { get; }
    public int PositionY { get; }
    public IEnumerable<MapLocation> ConnectedLocations => Area.GameMapState.GetConnectedLocations(this);
    public IEnumerable<Character> Characters => Area.GameMapState.GameState.Characters.GetCharactersAtLocation(this);


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
