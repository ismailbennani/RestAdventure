using RestAdventure.Core.Interactions;
using RestAdventure.Core.StaticObjects;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Jobs;

public class HarvestInteraction : Interaction
{
    public HarvestInteraction(Job job, JobHarvest harvest)
    {
        Job = job;
        Harvest = harvest;
    }

    public override string Name => $"{Job.Name}-{Harvest.Name}";
    public Job Job { get; }
    public JobHarvest Harvest { get; }

    protected override Maybe CanInteractInternal(IInteractingEntity source, IInteractibleEntity target)
    {
        if (source is not IGameEntityWithJobs entityWithJobs)
        {
            return "Source doesn't have jobs";
        }

        JobInstance? job = entityWithJobs.Jobs.Get(Job);
        if (job == null || Harvest.Level > job.Progression.Level)
        {
            return "Source doesn't fulfill the conditions";
        }

        if (target is not StaticObjectInstance staticObjectInstance || !Harvest.Targets.Contains(staticObjectInstance.Object))
        {
            return "Entity cannot be harvested";
        }

        return true;
    }

    protected override Task<Maybe<InteractionInstance>> InstantiateInteractionInternalAsync(IInteractingEntity source, IInteractibleEntity target) =>
        Task.FromResult<Maybe<InteractionInstance>>(new HarvestInteractionInstance((IGameEntityWithJobs)source, this, target));
}
