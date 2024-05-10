using RestAdventure.Core.Extensions;

namespace SandboxGame.Generation.Terraforming;

public class ErodedIslandGenerator : LandGenerator
{
    public ErodedIslandGenerator(int width, int height, double targetErosionRatio)
    {
        Width = width;
        Height = height;
        TargetErosionRatio = targetErosionRatio;
    }

    public int Width { get; }
    public int Height { get; }
    public double TargetErosionRatio { get; }

    public override Land Generate()
    {
        if (TargetErosionRatio >= 1)
        {
            return Land.Empty;
        }

        int xMin = -Width / 2;
        int yMin = -Height / 2;
        int xMax = xMin + Width - 1;
        int yMax = yMin + Height - 1;

        if (TargetErosionRatio <= 0)
        {
            return new Land
            {
                Locations = Enumerable.Range(xMin, Width).SelectMany(x => Enumerable.Range(yMin, Height).Select(y => (x, y))).ToArray(),
                XMin = xMin,
                XMax = xMax,
                YMin = yMin,
                YMax = yMax
            };
        }

        Random random = Random.Shared;
        int totalCells = Width * Height;
        int cellsToErode = (int)Math.Floor(TargetErosionRatio * totalCells);

        HashSet<(int X, int Y)> canBeEroded = new();
        for (int x = xMin; x <= xMax; x++)
        {
            canBeEroded.Add((x, yMin));
            canBeEroded.Add((x, yMax));
        }
        for (int y = yMin; y < yMax; y++)
        {
            canBeEroded.Add((xMin, y));
            canBeEroded.Add((xMax, y));
        }

        HashSet<(int X, int Y)> erodedCells = new();
        for (int i = 0; i < cellsToErode; i++)
        {
            if (canBeEroded.Count == 0)
            {
                break;
            }

            (int X, int Y) toErode = random.Choose(canBeEroded);

            Erode(erodedCells, canBeEroded, toErode, xMin, xMax, yMin, yMax);
        }

        HashSet<(int X, int Y)> locations = Enumerable.Range(xMin, Width)
            .SelectMany(x => Enumerable.Range(yMin, Height).Select(y => (x, y)))
            .Where(pos => !erodedCells.Contains(pos))
            .ToHashSet();

        IReadOnlyCollection<(int X, int Y)> biggestComponent = LandGraph.ComputeConnectedComponents(locations).MaxBy(c => c.Count) ?? Array.Empty<(int, int)>();

        return new Land
        {
            Locations = biggestComponent,
            XMin = biggestComponent.Min(l => l.X),
            XMax = biggestComponent.Max(l => l.X),
            YMin = biggestComponent.Min(l => l.Y),
            YMax = biggestComponent.Max(l => l.Y)
        };
    }

    void RemoveDisconnectedComponents(HashSet<(int x, int y)> locations) => throw new NotImplementedException();

    static void Erode(HashSet<(int X, int Y)> erodedCells, HashSet<(int X, int Y)> canBeEroded, (int X, int Y) toErode, int xMin, int xMax, int yMin, int yMax)
    {
        canBeEroded.Remove(toErode);
        erodedCells.Add(toErode);

        MarkForErosion(erodedCells, canBeEroded, (toErode.X - 1, toErode.Y), xMin, xMax, yMin, yMax);
        MarkForErosion(erodedCells, canBeEroded, (toErode.X + 1, toErode.Y), xMin, xMax, yMin, yMax);
        MarkForErosion(erodedCells, canBeEroded, (toErode.X, toErode.Y - 1), xMin, xMax, yMin, yMax);
        MarkForErosion(erodedCells, canBeEroded, (toErode.X, toErode.Y + 1), xMin, xMax, yMin, yMax);
    }

    static void MarkForErosion(HashSet<(int X, int Y)> erodedCells, HashSet<(int X, int Y)> canBeEroded, (int X, int Y) toMark, int xMin, int xMax, int yMin, int yMax)
    {
        if (toMark.X < xMin || toMark.X > xMax || toMark.Y < yMin || toMark.Y > yMax)
        {
            return;
        }

        if (erodedCells.Contains(toMark))
        {
            return;
        }

        canBeEroded.Add((toMark.X, toMark.Y));
    }
}
