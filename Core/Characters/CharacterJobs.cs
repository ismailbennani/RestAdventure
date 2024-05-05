using RestAdventure.Core.Characters.Notifications;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Characters;

public class CharacterJobs
{
    readonly Character _character;
    readonly Dictionary<JobId, JobInstance> _jobs = new();

    public CharacterJobs(Character character)
    {
        _character = character;
    }

    public async Task<JobInstance> LearnAsync(Job job)
    {
        JobInstance jobInstance = new(job);
        _jobs[job.Id] = jobInstance;

        jobInstance.LeveledUp += (_, args) => _character.Team.GameCharacters.GameState.Publisher.Publish(
                new CharacterJobLeveledUp
                {
                    Character = _character,
                    Job = jobInstance.Job,
                    OldLevel = args.OldLevel,
                    NewLevel = args.NewLevel
                }
            )
            .Wait();

        await _character.Team.GameCharacters.GameState.Publisher.Publish(new CharacterLearnedJob { Character = _character, Job = job });

        return jobInstance;
    }

    public JobInstance? Get(Job job) => _jobs.GetValueOrDefault(job.Id);
}
