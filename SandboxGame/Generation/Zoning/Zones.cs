namespace SandboxGame.Generation.Zoning;

public class Zone
{
    public required string Name { get; init; }
    public required int Level { get; init; }
    public required int PartitionIndex { get; init; }
    public required (int X, int Y) PartitionCenter { get; init; }
}
