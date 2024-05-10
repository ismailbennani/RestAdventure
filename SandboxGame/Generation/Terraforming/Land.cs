namespace SandboxGame.Generation.Terraforming;

public class Land
{
    public static Land Empty { get; } = new()
    {
        Locations = Array.Empty<(int, int)>(),
        XMin = 0,
        XMax = 0,
        YMin = 0,
        YMax = 0
    };

    public required IReadOnlyCollection<(int X, int Y)> Locations { get; init; }
    public int XMin { get; init; }
    public int XMax { get; init; }
    public int YMin { get; init; }
    public int YMax { get; init; }
}
