namespace SandboxGame.Generation.Partitioning;

public class Partition
{
    public static Partition Empty { get; } = new([[]], []);

    readonly Dictionary<int, Dictionary<int, int>> _subsets;
    readonly IReadOnlyList<(int X, int Y)> _subsetsCenters;

    public Partition(IReadOnlyList<IReadOnlyCollection<(int X, int Y)>> subsets, IReadOnlyList<(int X, int Y)> subsetsCenters)
    {
        Count = subsets.Count;
        _subsets = subsets.SelectMany((z, i) => z.Select(p => new { Position = p, Zone = i }))
            .GroupBy(x => x.Position.X)
            .ToDictionary(x => x.Key, x => x.ToDictionary(x => x.Position.Y, x => x.Zone));
        _subsetsCenters = subsetsCenters;
    }

    public int Count { get; }

    public (int X, int Y) GetSubsetCenter(int subset) => _subsetsCenters.ElementAtOrDefault(subset);

    public int? GetSubset((int X, int Y) position) => TryGetSubset(position, out int subset) ? subset : null;

    public bool TryGetSubset((int X, int Y) position, out int subset)
    {
        if (!_subsets.TryGetValue(position.X, out Dictionary<int, int>? d))
        {
            subset = default;
            return false;
        }

        return d.TryGetValue(position.Y, out subset);
    }
}
