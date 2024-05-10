using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Terraforming;

namespace SandboxGame.Generation.Zoning;

public abstract class ZonesGenerator
{
    public abstract IReadOnlyList<Zone> Generate(Land land, Partition partition);
}
