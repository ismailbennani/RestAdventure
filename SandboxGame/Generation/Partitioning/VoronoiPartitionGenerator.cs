using Microsoft.Extensions.Logging;
using RestAdventure.Core.Extensions;
using SandboxGame.Generation.Terraforming;
using SandboxGame.MyMath;

namespace SandboxGame.Generation.Partitioning;

public class VoronoiPartitionGenerator : PartitionGenerator
{
    readonly ILogger<VoronoiPartitionGenerator> _logger;

    public VoronoiPartitionGenerator(int subsetsCount, ILogger<VoronoiPartitionGenerator> logger)
    {
        _logger = logger;
        SubsetsCount = subsetsCount;
    }

    int SubsetsCount { get; }

    public override Partition Generate(Land land)
    {
        if (land.Locations.Count == 0)
        {
            return Partition.Empty;
        }

        Random random = Random.Shared;

        (int, int)[] subsetsCenters = GenerateSubsetsCenters(random, land, SubsetsCount).ToArray();
        if (subsetsCenters.Length < SubsetsCount)
        {
            _logger.LogWarning("Could not generate enough subsets: ran out of fuel");
        }

        Dictionary<(int X, int Y), int> subsets = new();

        foreach ((int X, int Y) location in land.Locations)
        {
            subsets[location] = ComputeSubset(location, subsetsCenters);
        }

        (int, int)[][] subsetsArr = subsets.GroupBy(kv => kv.Value).OrderBy(g => g.Key).Select(g => g.Select(kv => kv.Key).ToArray()).ToArray();
        return new Partition(subsetsArr, subsetsCenters);
    }

    static IEnumerable<(int, int)> GenerateSubsetsCenters(Random random, Land land, int count)
    {
        HashSet<(int, int)> subsetsCenters = [];
        int fuel = count * 1000;

        while (subsetsCenters.Count < count && fuel > 0)
        {
            (int X, int Y) subsetCenter = random.Choose(land.Locations);
            subsetsCenters.Add(subsetCenter);

            fuel--;
        }

        return subsetsCenters;
    }

    static int ComputeSubset((int X, int Y) location, IReadOnlyList<(int, int)> subsetCenters)
    {
        int minDist = int.MaxValue;
        int subset = -1;

        for (int i = 0; i < subsetCenters.Count; i++)
        {
            int dist = Distance.L1(location, subsetCenters[i]);
            if (dist < minDist)
            {
                minDist = dist;
                subset = i;
            }
        }

        return subset;
    }
}
