using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using RestAdventure.Core.Extensions;
using SandboxGame.Generation.Terraforming;

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

        (int X, int Y)[] subsetsCenters = GenerateSubsetsCenters(random, land, SubsetsCount).ToArray();
        if (subsetsCenters.Length < SubsetsCount)
        {
            _logger.LogWarning("Could not generate enough subsets: ran out of fuel");
        }

        Dictionary<(int, int), HashSet<(int, int)>> adjacency = LandGraph.ComputeLandAdjacency(land);
        ConcurrentDictionary<(int, int), int> subsets = new();

        ParallelLoopResult result = Parallel.ForEach(land.Locations, l => subsets[l] = ComputeSubsetForLocation(subsetsCenters, l, adjacency));
        if (!result.IsCompleted)
        {
            throw new InvalidOperationException("Computing subsets for locations didn't compelte");
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

    static int ComputeSubsetForLocation(IEnumerable<(int X, int Y)> subsetsCenters, (int X, int Y) location, Dictionary<(int, int), HashSet<(int, int)>> adjacency) =>
        subsetsCenters.Select(
                (c, i) => new
                {
                    Distance = LandGraph.AStartDistance(location, c, adjacency),
                    Subset = i
                }
            )
            .MinBy(x => x.Distance)
            ?.Subset
        ?? 0;
}
