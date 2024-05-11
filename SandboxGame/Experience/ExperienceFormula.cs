namespace SandboxGame.Experience;

public abstract class ExperienceFormula
{
    public abstract int ComputeExperience(int level);
    public abstract int ComputeLevel(int experience);
}

public static class ExperienceFormulaMappingExtensions
{
    public static IReadOnlyList<int> ComputeLevelCaps(this ExperienceFormula formula, int from, int to) =>
        Enumerable.Range(from, to - from + 1).Select(formula.ComputeExperience).ToArray();
}
