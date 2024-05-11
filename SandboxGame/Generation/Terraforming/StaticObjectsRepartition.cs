using RestAdventure.Core.Entities.StaticObjects;

namespace SandboxGame.Generation.Terraforming;

public abstract class StaticObjectsRepartition
{
    protected StaticObjectsRepartition(StaticObject target)
    {
        Target = target;
    }

    /// <summary>
    ///     The objects corresponding to the resource allocation
    /// </summary>
    public StaticObject Target { get; }

    /// <summary>
    ///     Return a score for the given location.
    ///     This score represent the expected quantity of target instances on the location.
    /// </summary>
    public abstract IReadOnlyCollection<(StaticObject Obj, int Count)> GetObjectsInPartition(int partition);
}
