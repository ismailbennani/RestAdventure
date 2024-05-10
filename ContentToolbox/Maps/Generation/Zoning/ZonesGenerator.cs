using ContentToolbox.Maps.Generation.LandGeneration;

namespace ContentToolbox.Maps.Generation.Zoning;

public abstract class ZonesGenerator
{
    public abstract Zones Generate(Land land);
}
