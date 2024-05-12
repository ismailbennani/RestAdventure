namespace RestAdventure.Core.Utils;

public static class Distance
{
    public static int L1((int X, int Y) p1, (int X, int Y) p2) => Math.Abs(p2.X - p1.X) + Math.Abs(p2.Y - p1.Y);
    public static int L2Sqr((int X, int Y) p1, (int X, int Y) p2) => (p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y);
}
