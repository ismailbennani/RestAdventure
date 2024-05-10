namespace SandboxGame.Generation.Partitioning;

public class Partition
{
    public static Partition Empty { get; } = new([[]]);

    readonly IReadOnlyDictionary<int, Dictionary<int, int>> _subsets;

    public Partition(IReadOnlyList<IReadOnlyCollection<(int X, int Y)>> partitions)
    {
        Count = partitions.Count;
        _subsets = partitions.SelectMany((z, i) => z.Select(p => new { Position = p, Zone = i }))
            .GroupBy(x => x.Position.X)
            .ToDictionary(x => x.Key, x => x.ToDictionary(x => x.Position.Y, x => x.Zone));
    }

    public int Count { get; }

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
