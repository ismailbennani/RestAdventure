using SandboxGame.MyMath;

namespace SandboxGame.Generation.Terraforming;

public static class LandGraph
{
    public static Dictionary<(int, int), HashSet<(int, int)>> ComputeLandAdjacency(Land land)
    {
        Dictionary<(int, int), HashSet<(int, int)>> result = new();

        foreach ((int X, int Y) position in land.Locations)
        {
            (int X, int) top = (position.X, position.Y + 1);
            if (land.Locations.Contains(top))
            {
                AddResult(position, top);
            }

            (int X, int) bottom = (position.X, position.Y - 1);
            if (land.Locations.Contains(bottom))
            {
                AddResult(position, bottom);
            }

            (int, int Y) right = (position.X + 1, position.Y);
            if (land.Locations.Contains(right))
            {
                AddResult(position, right);
            }

            (int, int Y) left = (position.X - 1, position.Y);
            if (land.Locations.Contains(left))
            {
                AddResult(position, left);
            }
        }

        return result;

        void AddResult((int, int) position, (int, int) adjacentPosition)
        {
            if (!result.TryGetValue(position, out HashSet<(int, int)>? value))
            {
                value = [];
                result[position] = value;
            }

            value.Add(adjacentPosition);
        }
    }

    public static Dictionary<(int, int), int> ComputeDistancesFromSource((int, int) source, IEnumerable<(int, int)> targets, Dictionary<(int, int), HashSet<(int, int)>> adjacency)
    {
        Dictionary<(int, int), int> result = new();
        result[source] = 0;

        HashSet<(int, int)> vertices = [];
        foreach ((int, int) vertex in targets)
        {
            vertices.Add(vertex);
        }

        while (vertices.Count > 0)
        {
            (int, int) vertex = vertices.Where(v => result.ContainsKey(v)).MinBy(v => result[v]);
            vertices.Remove(vertex);

            foreach ((int, int) adjacentVertex in adjacency[vertex])
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

    public static int? AStartDistance((int, int) source, (int, int) target, Dictionary<(int, int), HashSet<(int, int)>> adjacency)
    {
        HashSet<(int, int)> inToProcess = new();
        inToProcess.Add(source);

        PriorityQueue<(int, int), double> toProcess = new();
        toProcess.Enqueue(source, Distance.L1(source, target));

        Dictionary<(int, int), int> distances = new();
        distances[source] = 0;

        while (toProcess.TryDequeue(out (int, int) next, out _))
        {
            inToProcess.Remove(next);

            if (next == target)
            {
                return distances[next];
            }

            foreach ((int, int) adjacentNode in adjacency[next])
            {
                int alternateDistance = distances[next] + 1;
                if (!distances.TryGetValue(adjacentNode, out int distance) || alternateDistance < distance)
                {
                    distances[adjacentNode] = alternateDistance;
                    if (inToProcess.Add(adjacentNode))
                    {
                        toProcess.Enqueue(adjacentNode, alternateDistance + Distance.L1(adjacentNode, target));
                    }
                }
            }
        }

        return null;
    }

    public static IReadOnlyCollection<IReadOnlyCollection<(int X, int Y)>> ComputeConnectedComponents(IReadOnlyCollection<(int X, int Y)> positions)
    {
        List<HashSet<(int X, int Y)>> components = [];

        foreach ((int X, int Y) position in positions)
        {
            if (components.Any(c => c.Contains(position)))
            {
                continue;
            }

            components.Add(ComputeConnectedComponent(position, positions));
        }

        return components;
    }

    static HashSet<(int X, int Y)> ComputeConnectedComponent((int X, int Y) position, IReadOnlyCollection<(int X, int Y)> positions)
    {
        HashSet<(int X, int Y)> result = new();

        Queue<(int X, int Y)> queue = new();
        queue.Enqueue(position);

        while (queue.TryDequeue(out (int X, int Y) current))
        {
            if (!result.Add(current))
            {
                continue;
            }

            EnqueueIfPositionExists(queue, (current.X + 1, current.Y), positions);
            EnqueueIfPositionExists(queue, (current.X - 1, current.Y), positions);
            EnqueueIfPositionExists(queue, (current.X, current.Y + 1), positions);
            EnqueueIfPositionExists(queue, (current.X, current.Y - 1), positions);
        }

        return result;
    }

    static void EnqueueIfPositionExists(Queue<(int X, int Y)> queue, (int X, int Y) position, IReadOnlyCollection<(int X, int Y)> positions)
    {
        if (positions.Contains(position))
        {
            queue.Enqueue(position);
        }
    }
}
