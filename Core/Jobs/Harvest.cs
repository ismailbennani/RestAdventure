using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Jobs;

public static class Harvest
{
    public static Maybe CanHarvestWithCorrectTool(this IJobInstance jobInstance, JobHarvest harvest, StaticObject staticObject)
    {
        if (!harvest.Targets.Contains(staticObject))
        {
            return "Entity doesn't match the harvest";
        }

        if (harvest.Level > jobInstance.Progression.Level)
        {
            return "Job level too low";
        }

        return true;
    }

    public static Maybe CanHarvest(this IJobInstance jobInstance, JobHarvest harvest, StaticObject staticObject, Item? tool = null)
    {
        Maybe canHarvest = CanHarvestWithCorrectTool(jobInstance, harvest, staticObject);
        if (!canHarvest.Success)
        {
            return canHarvest.WhyNot;
        }

        if (harvest.Tool != null)
        {
            if (tool == null)
            {
                return "Requires a tool";
            }

            if (harvest.Tool != tool.Category)
            {
                return "Tool in use doesn't match the required tool";
            }
        }

        return true;
    }
}
