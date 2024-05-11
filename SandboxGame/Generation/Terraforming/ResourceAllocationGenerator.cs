using RestAdventure.Core.Entities.StaticObjects;
using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Shaping;
using SandboxGame.Generation.Zoning;

namespace SandboxGame.Generation.Terraforming;

public abstract class ResourceAllocationGenerator
{
    public abstract IReadOnlyDictionary<(int X, int Y), IReadOnlyCollection<(StaticObject Object, double Count)>> Generate(
        Land land,
        Partition partition,
        IReadOnlyList<Zone> zones
    );
}
