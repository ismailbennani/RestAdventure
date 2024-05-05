using RestAdventure.Core.Resources;

namespace RestAdventure.Core.Maps.Areas;

public record MapAreaId(Guid Guid) : ResourceId(Guid);

public class MapArea : GameResource<MapAreaId>
{
    public MapArea() : base(new MapAreaId(Guid.NewGuid())) { }

    public required string Name { get; init; }

    public override string ToString() => $"Area {Name} ({Id})";
}
