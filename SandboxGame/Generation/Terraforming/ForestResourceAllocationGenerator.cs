using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Extensions;
using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Shaping;
using SandboxGame.Generation.Zoning;
using SandboxGame.MyMath;

namespace SandboxGame.Generation.Terraforming;

public class ForestResourceAllocationGenerator : ResourceAllocationGenerator
{
    public ForestResourceAllocationGenerator(IEnumerable<WeightedResource> weightedResources)
    {
        WeightedResources = weightedResources.ToArray();
    }

    public IReadOnlyCollection<WeightedResource> WeightedResources { get; }

    /// <summary>
    ///     Hint: distance from center at which density is divided by half
    /// </summary>
    public required double ForestSize { get; init; }

    /// <summary>
    ///     Hint: expected number of objects at center position
    /// </summary>
    public required double ForestDensity { get; init; }

    /// <summary>
    ///     Hint: For a density of 1 and a resource weight of 1, this is the distance to center at which spawning should stop
    /// </summary>
    public int Cutoff { get; init; } = 10;

    public override IReadOnlyDictionary<(int X, int Y), IReadOnlyCollection<(StaticObject Object, int Count)>> Generate(Land land, Partition partition, IReadOnlyList<Zone> zones)
    {
        (int X, int Y) forestCenter = Random.Shared.Choose(land.Locations);

        Dictionary<(int, int), IReadOnlyCollection<(StaticObject, int)>> result = new();

        double cutoffWeight = 1.0 / (1 + Cutoff / ForestSize);

        foreach ((int X, int Y) location in land.Locations)
        {
            double distanceWeight = 1.0 / (1 + Distance.L1(location, forestCenter) / ForestSize);
            double forestWeight = ForestDensity * distanceWeight;

            List<(StaticObject, int)> locationResult = [];
            foreach (WeightedResource weightedResource in WeightedResources)
            {
                double weight = forestWeight * weightedResource.Weight;
                if (weight < cutoffWeight)
                {
                    continue;
                }

                locationResult.Add((weightedResource.Object, (int)weight));
            }

            result[location] = locationResult;
        }

        return result;
    }

    public class WeightedResource
    {
        public required StaticObject Object { get; init; }
        public required double Weight { get; init; }
    }
}
