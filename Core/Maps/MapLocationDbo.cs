using RestAdventure.Core.Characters;
using Xtensive.Orm;

namespace RestAdventure.Core.Maps;

[HierarchyRoot]
[Index(nameof(PositionX), nameof(PositionY))]
public class MapLocationDbo : Entity
{
    public MapLocationDbo(MapAreaDbo area, int x, int y)
    {
        Area = area;
        PositionX = x;
        PositionY = y;
    }

    [Key]
    [Field]
    public Guid Id { get; private set; }

    [Field]
    public MapAreaDbo Area { get; private set; }

    [Field]
    public int PositionX { get; private set; }

    [Field]
    public int PositionY { get; private set; }

    [Field]
    [Association(nameof(ConnectedLocations), OnOwnerRemove = OnRemoveAction.Clear, OnTargetRemove = OnRemoveAction.Clear)]
    public EntitySet<MapLocationDbo> ConnectedLocations { get; private set; }

    [Field]
    [Association(nameof(CharacterDbo.Location), OnOwnerRemove = OnRemoveAction.Deny, OnTargetRemove = OnRemoveAction.Clear)]
    public EntitySet<CharacterDbo> Characters { get; private set; }
}
