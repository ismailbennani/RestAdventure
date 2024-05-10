using SandboxGame.Generation.Terraforming;

namespace SandboxGame.Generation.Zoning;

public abstract class ZonesGenerator
{
    public abstract Zones Generate(Land land);
}
