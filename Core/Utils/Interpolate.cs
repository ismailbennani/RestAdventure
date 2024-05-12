namespace SandboxGame.MyMath;

public static class Interpolate
{
    public static double LinearUnclamped(double x1, double y1, double x2, double y2, double t)
    {
        double diffX = x2 - x1;
        if (diffX == 0)
        {
            throw new InvalidOperationException("Expected x values to be different");
        }

        double slope = (y2 - y1) / diffX;
        return y1 + (t - x1) * slope;
    }
}
