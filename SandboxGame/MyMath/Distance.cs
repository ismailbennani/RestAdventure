namespace SandboxGame.MyMath;

public static class Distance
{
    public static int L1((int X, int Y) p1, (int X, int Y) p2) => Math.Abs(p2.X - p1.X) + Math.Abs(p2.Y - p1.Y);
}
