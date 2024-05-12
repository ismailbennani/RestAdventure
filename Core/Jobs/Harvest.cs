using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Jobs;

public static class Harvest
{
    public static IEnumerable<JobHarvest> GetAvailableHarvests(this IJobInstance jobInstance, StaticObject staticObject, Item? tool = null) =>
        jobInstance.Job.Harvests.Where(h => CanHarvest(jobInstance, h, staticObject, tool));

    public static Maybe CanHarvest(this IJobInstance jobInstance, JobHarvest harvest, StaticObject staticObject, Item? tool = null)
    {
        if (!harvest.Targets.Contains(staticObject))
        {
            return "The object doesn't match the harvest";
        }

        if (harvest.Level > jobInstance.Progression.Level)
        {
            return "Job level too low";
        }

        if (harvest.Tool != null)
        {
            if (tool == null)
            {
                return "The harvest requires a tool";
            }

            if (harvest.Tool != tool.Category)
            {
                return "The tool in use doesn't match the harvest";
            }
        }

        return true;
    }
}
