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
    ///     Hint: Distance at which spawning stops
    /// </summary>
    public int DistanceCutoff { get; init; } = 10;

    public override IReadOnlyDictionary<(int X, int Y), IReadOnlyCollection<(StaticObject Object, double Count)>> Generate(
        Land land,
        Partition partition,
        IReadOnlyList<Zone> zones
    )
    {
        (int X, int Y) forestCenter = Random.Shared.Choose(land.Locations);

        Dictionary<(int, int), IReadOnlyCollection<(StaticObject, double)>> result = new();

        foreach ((int X, int Y) location in land.Locations)
        {
            int distance = Distance.L1(location, forestCenter);
            if (distance > DistanceCutoff)
            {
                continue;
            }

            double distanceWeight = 1.0 / (1 + distance / ForestSize);
            double forestWeight = ForestDensity * distanceWeight;

            int zoneLevel = GetZoneLevel(location, partition, zones);
            Dictionary<StaticObject, double> weights = WeightedResources.ToDictionary(r => r.Object, r => ComputeRelativeWeightForLevel(r.WeightsByZoneLevel, zoneLevel));
            double sumRelativeWeights = weights.Values.Sum();

            List<(StaticObject, double)> locationResult = [];
            foreach (WeightedResource weightedResource in WeightedResources)
            {
                double weight = forestWeight * weights[weightedResource.Object] / sumRelativeWeights;
                locationResult.Add((weightedResource.Object, weight));
            }

            result[location] = locationResult;
        }

        return result;
    }

    static int GetZoneLevel((int X, int Y) location, Partition partition, IReadOnlyList<Zone> zones) =>
        partition.TryGetSubset(location, out int subsetIndex) ? zones.SingleOrDefault(z => z.PartitionIndex == subsetIndex)?.Level ?? 0 : 0;

    static double ComputeRelativeWeightForLevel(Dictionary<int, double> relativeWeightsByLevel, int zoneLevel)
    {
        switch (relativeWeightsByLevel.Count)
        {
            case 0:
                return 0;
            case 1:
                return relativeWeightsByLevel.Single().Value;
        }

        if (relativeWeightsByLevel.TryGetValue(zoneLevel, out double value))
        {
            return value;
        }

        KeyValuePair<int, double>? biggestLower = null;
        KeyValuePair<int, double>? next = null;

        foreach (KeyValuePair<int, double> entry in relativeWeightsByLevel)
        {
            if (entry.Key < zoneLevel)
            {
                biggestLower = entry;
            }
            else if (!next.HasValue)
            {
                next = entry;
            }
        }

        if (biggestLower.HasValue && next.HasValue)
        {
            return Interpolate.LinearUnclamped(biggestLower.Value.Key, biggestLower.Value.Value, next.Value.Key, next.Value.Value, zoneLevel);
        }

        if (biggestLower.HasValue)
        {
            return biggestLower.Value.Value;
        }

        if (next.HasValue)
        {
            return next.Value.Value;
        }

        throw new InvalidOperationException("SHOULD NOT HAPPEN");
    }

    public class WeightedResource
    {
        public required StaticObject Object { get; init; }
        public required Dictionary<int, double> WeightsByZoneLevel { get; init; }
    }
}
