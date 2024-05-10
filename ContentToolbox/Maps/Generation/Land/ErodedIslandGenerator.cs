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
            canBeEroded.Remove(toErode);
            erodedCells.Add(toErode);

            canBeEroded.Add((toErode.X - 1, toErode.Y));
            canBeEroded.Add((toErode.X + 1, toErode.Y));
            canBeEroded.Add((toErode.X, toErode.Y - 1));
            canBeEroded.Add((toErode.X, toErode.Y + 1));
        }

        return new Land
        {
            Locations = Enumerable.Range(xMin, Width).SelectMany(x => Enumerable.Range(yMin, Height).Select(y => (x, y))).Where(pos => !erodedCells.Contains(pos)).ToArray(),
            XMin = xMin,
            XMax = xMax,
            YMin = yMin,
            YMax = yMax
        };
    }
}
