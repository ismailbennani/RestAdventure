namespace SandboxGame.Generation.Zoning;

public class Zones
{
    public static Zones Empty { get; } = new([[]]);

    readonly Dictionary<(int X, int Y), int> _zoning;

    public Zones(IReadOnlyList<IReadOnlyCollection<(int X, int Y)>> zoning)
    {
        Count = zoning.Count;
        _zoning = zoning.SelectMany((z, i) => z.Select(p => new { Position = p, Zone = i })).ToDictionary(x => x.Position, x => x.Zone);
    }

    public int Count { get; }

    public int? GetZone((int, int) position) => _zoning.GetValueOrDefault(position);
}
