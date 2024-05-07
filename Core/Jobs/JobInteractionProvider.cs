using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Interactions.Providers;
using RestAdventure.Core.StaticObjects;

namespace RestAdventure.Core.Jobs;

public class JobInteractionProvider : IInteractionProvider
{
    readonly Dictionary<string, Interaction> _cache = new();

    public IEnumerable<Interaction> GetAvailableInteractions(Character character, IGameEntity entity)
    {
        if (entity is not StaticObjectInstance staticObjectInstance)
        {
            yield break;
        }

        foreach (JobInstance job in character.Jobs)
        {
            foreach (JobHarvest harvest in job.Harvests)
            {
                if (harvest.Targets.Contains(staticObjectInstance.Object))
                {
                    yield return GetHarvestInteraction(job.Job, harvest);
                }
            }
        }
    }

    Interaction GetHarvestInteraction(Job job, JobHarvest harvest)
    {
        string key = $"{job.Name}-{harvest.Name}";
        if (!_cache.TryGetValue(key, out Interaction? interaction))
        {
            interaction = new HarvestInteraction(job, harvest);
            _cache[key] = interaction;
        }

        return interaction;
    }
}
