using ContentToolbox.Noise;
using RestAdventure.Core.Entities.StaticObjects;
using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Shaping;
using SandboxGame.Generation.Zoning;
using SandboxGame.MyMath;

namespace SandboxGame.Generation.Terraforming;

public class NoiseResourceAllocationGenerator : ResourceAllocationGenerator
{
    public NoiseResourceAllocationGenerator(IEnumerable<WeightedResource> weightedResources, Noise2D noise)
    {
        Noise = noise;
        WeightedResources = weightedResources.ToArray();
    }

    public IReadOnlyCollection<WeightedResource> WeightedResources { get; }
    public Noise2D Noise { get; }

    /// <summary>
    ///     Hint: max expected number of objects per location
    /// </summary>
    public required double Coefficient { get; init; }

    /// <summary>
    ///     Noise values under this will be ignored
    /// </summary>
    public double NoiseCutoff { get; init; } = 0.5;

    public override IReadOnlyDictionary<(int X, int Y), IReadOnlyCollection<(StaticObject Object, double Count)>> Generate(
        Land land,
        Partition partition,
        IReadOnlyList<Zone> zones
    )
    {
        Random shared = Random.Shared;
        Dictionary<(int, int), IReadOnlyCollection<(StaticObject, double)>> result = new();

        foreach ((int X, int Y) location in land.Locations)
        {
            double noise = Noise.Get(location.X, location.Y);
            if (noise < NoiseCutoff)
            {
                continue;
            }

            double forestWeight = Coefficient * noise;

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
