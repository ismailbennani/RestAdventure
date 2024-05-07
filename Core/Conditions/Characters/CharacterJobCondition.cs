﻿using RestAdventure.Core.Characters;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Conditions.Characters;

public class CharacterJobCondition : ICharacterCondition
{
    public CharacterJobCondition(Job job, int minLevel = 1)
    {
        Job = job;
        MinLevel = minLevel;
    }

    public Job Job { get; }
    public int MinLevel { get; }

    public bool Evaluate(Character character)
    {
        JobInstance? jobInstance = character.Jobs.Get(Job);
        return jobInstance?.Progression.Level >= MinLevel;
    }
}
