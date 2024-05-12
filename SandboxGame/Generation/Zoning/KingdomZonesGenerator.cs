using RestAdventure.Core.Utils;
using SandboxGame.Generation.Partitioning;
using SandboxGame.Generation.Shaping;

namespace SandboxGame.Generation.Zoning;

/// <summary>
///     <list type="bullet">
///         <item>Central level 0 zone</item>
///         <item>Level increases as zones get further and further away</item>
///     </list>
/// </summary>
public class KingdomZonesGenerator : ZonesGenerator
{
    public override IReadOnlyList<Zone> Generate(Land land, Partition partition)
    {
        if (land.Locations.Count == 0 || partition.Count == 0)
        {
            return Array.Empty<Zone>();
        }

        Dictionary<int, HashSet<int>> adjacency = ComputePartitionAdjacency(land, partition);

        var centralPosition = land.Locations.Select(l => new { Position = l, Partition = partition.GetSubset(l) })
            .Where(x => x.Partition.HasValue)
            .MinBy(l => Distance.L1(l.Position, (0, 0)));
        int centralPartition = centralPosition!.Partition!.Value;

        Dictionary<int, int> distanceToCentralPartition = ComputeDistancesToPartition(adjacency, centralPartition);

        return Enumerable.Range(0, partition.Count)
            .Select(i => new Zone { Name = $"Zone {i}", Level = distanceToCentralPartition[i] * 10, PartitionIndex = i, PartitionCenter = partition.GetSubsetCenter(i) })
            .ToArray();
    }

    static Dictionary<int, HashSet<int>> ComputePartitionAdjacency(Land land, Partition partition)
    {
        Dictionary<int, HashSet<int>> result = new();

        foreach ((int X, int Y) position in land.Locations)
        {
            if (!partition.TryGetSubset(position, out int positionPartition))
            {
                continue;
            }

            if (partition.TryGetSubset((position.X, position.Y + 1), out int topPartition) && topPartition != positionPartition)
            {
                AddAdjacentPartition(result, positionPartition, topPartition);
            }

            if (partition.TryGetSubset((position.X, position.Y - 1), out int bottomPartition) && bottomPartition != positionPartition)
            {
                AddAdjacentPartition(result, positionPartition, bottomPartition);
            }

            if (partition.TryGetSubset((position.X + 1, position.Y), out int rightPartition) && rightPartition != positionPartition)
            {
                AddAdjacentPartition(result, positionPartition, rightPartition);
            }

            if (partition.TryGetSubset((position.X - 1, position.Y), out int leftPartition) && leftPartition != positionPartition)
            {
                AddAdjacentPartition(result, positionPartition, leftPartition);
            }
        }

        return result;
    }

    static void AddAdjacentPartition(Dictionary<int, HashSet<int>> adjacency, int partition, int adjacentPartition)
    {
        if (!adjacency.TryGetValue(partition, out HashSet<int>? partitionAdjacency))
        {
            adjacency[partition] = [adjacentPartition];
        }
        else
        {
            partitionAdjacency.Add(adjacentPartition);
        }

        if (!adjacency.TryGetValue(adjacentPartition, out HashSet<int>? adjacentPartitionAdjacency))
        {
            adjacency[adjacentPartition] = [partition];
        }
        else
        {
            adjacentPartitionAdjacency.Add(partition);
        }
    }

    static Dictionary<int, int> ComputeDistancesToPartition(Dictionary<int, HashSet<int>> adjacency, int source)
    {
        Dictionary<int, int> result = new();
        result[source] = 0;

        HashSet<int> vertices = new();
        foreach (int vertex in adjacency.Keys)
        {
            vertices.Add(vertex);
        }

        while (vertices.Count > 0)
        {
            int vertex = vertices.Where(v => result.ContainsKey(v)).MinBy(v => result[v]);
            vertices.Remove(vertex);

            foreach (int adjacentVertex in adjacency[vertex])
            {
                if (!vertices.Contains(adjacentVertex))
                {
                    continue;
                }

                int alternativePathLength = result[vertex] + 1;
                if (!result.ContainsKey(adjacentVertex) || alternativePathLength < result[adjacentVertex])
                {
                    result[adjacentVertex] = alternativePathLength;
                }
            }
        }

        return result;
    }
}
