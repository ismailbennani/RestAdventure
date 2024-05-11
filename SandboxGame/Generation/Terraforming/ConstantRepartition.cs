using RestAdventure.Core.Entities.StaticObjects;

namespace SandboxGame.Generation.Terraforming;

public class ConstantRepartition : StaticObjectsRepartition
{
    readonly int _count;

    public ConstantRepartition(StaticObject target, int count) : base(target)
    {
        _count = count;
    }

    public override IReadOnlyCollection<(StaticObject Obj, int Count)> GetObjectsInPartition(int partition) => [(Target, _count)];
}
