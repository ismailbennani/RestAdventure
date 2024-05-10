using RestAdventure.Core.Extensions;

namespace ContentToolbox.Maps.Generation.Land;

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
    public bool DetachPeninsulas { get; init; } = true;

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

        HashSet<(int x, int y)> locations = Enumerable.Range(xMin, Width)
            .SelectMany(x => Enumerable.Range(yMin, Height).Select(y => (x, y)))
            .Where(pos => !erodedCells.Contains(pos))
            .ToHashSet();

        if (DetachPeninsulas)
        {
            DoDetachPeninsulas(locations);
        }

        return new Land
        {
            Locations = locations,
            XMin = locations.Min(l => l.x),
            XMax = locations.Max(l => l.x),
            YMin = locations.Min(l => l.y),
            YMax = locations.Max(l => l.y)
        };
    }

    static void DoDetachPeninsulas(HashSet<(int x, int y)> locations)
    {
        List<(int, int)> toRemove = new();
        foreach ((int x, int y) in locations)
        {
            if (!locations.Contains((x + 1, y))
                && !locations.Contains((x - 1, y))
                && !locations.Contains((x, y + 1))
                && !locations.Contains((x, y - 1))
                && (locations.Contains((x + 1, y + 1)) || locations.Contains((x - 1, y + 1)) || locations.Contains((x + 1, y - 1)) || locations.Contains((x - 1, y - 1))))
            {
                toRemove.Add((x, y));
            }
        }

        foreach ((int, int) position in toRemove)
        {
            locations.Remove(position);
        }
    }

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
