namespace SandboxGame.Generation.Partitioning;

public class Partition
{
    public static Partition Empty { get; } = new([[]]);

    readonly Dictionary<(int X, int Y), int> _partition;

    public Partition(IReadOnlyList<IReadOnlyCollection<(int X, int Y)>> partitions)
    {
        Count = partitions.Count;
        _partition = partitions.SelectMany((z, i) => z.Select(p => new { Position = p, Zone = i })).ToDictionary(x => x.Position, x => x.Zone);
    }

    public int Count { get; }

    public int? GetSubsetContaining((int, int) position) => _partition.GetValueOrDefault(position);
}
