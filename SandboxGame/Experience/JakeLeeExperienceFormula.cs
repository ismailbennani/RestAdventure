namespace SandboxGame.Experience;

/// <remarks>
///     (level / x) ^ y
///     See https://blog.jakelee.co.uk/converting-levels-into-xp-vice-versa/
/// </remarks>
public class JakeLeeExperienceFormula : ExperienceFormula
{
    public JakeLeeExperienceFormula(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; }
    public double Y { get; }

    public override int ComputeExperience(int level) =>
        Y switch
        {
            1 => (int)(level / X),
            2 => (int)(level * level / (X * X)),
            3 => (int)(level * level * level / (X * X * X)),
            _ => (int)Math.Pow(level / X, Y)
        };

    public override int ComputeLevel(int experience) =>
        Y switch
        {
            1 => (int)(X * experience),
            2 => (int)Math.Sqrt(X * experience),
            _ => (int)(X * Math.Pow(experience, 1.0 / Y))
        };
}
