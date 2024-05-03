using Xtensive.Orm;

namespace RestAdventure.Core.Maps;

[HierarchyRoot]
public class MapAreaDbo : Entity
{
    public MapAreaDbo(string name)
    {
        Name = name;
    }

    [Key]
    [Field]
    public Guid Id { get; private set; }

    [Field]
    public string Name { get; set; }

    [Field]
    [Association(nameof(MapLocationDbo.Area), OnTargetRemove = OnRemoveAction.Clear, OnOwnerRemove = OnRemoveAction.Cascade)]
    public EntitySet<MapLocationDbo> Locations { get; private set; }
}
