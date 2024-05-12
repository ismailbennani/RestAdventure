using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Jobs;

public static class CharacterHarvestExtensions
{
    public static Maybe CanHarvest(this ICharacter character, JobHarvest harvest, IStaticObjectInstance staticObjectInstance, ItemInstance? tool = null)
    {
        if (staticObjectInstance.Busy)
        {
            return "Target is busy";
        }

        IJobInstance? job = character.Jobs.SelectMany(j => j.Job.Harvests.Select(h => new { Job = j, Harvest = h })).FirstOrDefault(x => x.Harvest.Name == harvest.Name)?.Job;
        return job == null ? "Character doesn't have the required skill" : job.CanHarvest(harvest, staticObjectInstance.Object, tool?.Item);
    }

    public static Maybe CanHarvestWithCorrectTool(this ICharacter character, JobHarvest harvest, IStaticObjectInstance staticObjectInstance)
    {
        if (staticObjectInstance.Busy)
        {
            return "Target is busy";
        }

        IJobInstance? job = character.Jobs.SelectMany(j => j.Job.Harvests.Select(h => new { Job = j, Harvest = h })).FirstOrDefault(x => x.Harvest.Name == harvest.Name)?.Job;
        return job == null ? "Character doesn't have the required skill" : job.CanHarvestWithCorrectTool(harvest, staticObjectInstance.Object);
    }
}
