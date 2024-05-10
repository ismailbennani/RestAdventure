namespace ContentToolbox.Maps.Generation.Land;

public static class LandGraph
{
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
