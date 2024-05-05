using RestAdventure.Core.Maps.Areas;
using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Maps.Locations;

public record MapLocationId(Guid Guid) : ResourceId(Guid);

public class MapLocation : GameResource<MapLocationId>
{
    public MapLocation() : base(new MapLocationId(Guid.NewGuid()))
    {
    }

    /// <summary>
    ///     The area in which the location is
    /// </summary>
    public required MapArea Area { get; init; }

    /// <summary>
    ///     The X position of the location
    /// </summary>
    public required int PositionX { get; init; }

    /// <summary>
    ///     The Y position of the location
    /// </summary>
    public required int PositionY { get; init; }

    public override string ToString() => $"Location[{PositionX}, {PositionY}, {Area}] ({Id})";
}
