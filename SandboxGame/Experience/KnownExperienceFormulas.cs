﻿namespace SandboxGame.Experience;

public static class KnownExperienceFormulas
{
    public static readonly ExperienceFormula JobExperienceFormula = new JakeLeeExperienceFormula(0.3, 2);
    public static readonly IReadOnlyList<int> JobLevelCaps1To50 = JobExperienceFormula.ComputeLevelCaps(1, 50);

    public static readonly ExperienceFormula CharacterExperienceFormula = new JakeLeeExperienceFormula(0.07, 2);
    public static readonly IReadOnlyList<int> CharacterLevelCaps1To50 = CharacterExperienceFormula.ComputeLevelCaps(1, 50);
}
