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

    public override int ComputeExperience(int level) => (int)Math.Pow(level / X, Y);
    public override int ComputeLevel(int experience) => (int)(X * Math.Pow(experience, 1.0 / Y));
}
