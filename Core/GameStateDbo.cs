using Xtensive.Orm;

namespace RestAdventure.Core;

[HierarchyRoot]
public class GameStateDbo : Entity
{
    [Key]
    [Field]
    public Guid Id { get; private set; }

    [Field]
    public long Tick { get; set; }
}
