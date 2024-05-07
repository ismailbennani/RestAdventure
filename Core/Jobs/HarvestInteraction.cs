using RestAdventure.Core.Characters;
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

    protected override Task<Maybe> CanInteractInternalAsync(GameState state, Character character, IInteractibleEntity target)
    {
        JobInstance? job = character.Jobs.Get(Job);
        if (job == null || Harvest.Level > job.Progression.Level)
        {
            return Task.FromResult<Maybe>("Character doesn't fulfill the conditions");
        }

        if (target is not StaticObjectInstance staticObjectInstance || !Harvest.Targets.Contains(staticObjectInstance.Object))
        {
            return Task.FromResult<Maybe>("Entity cannot be harvested");
        }

        if (character.Location != target.Location)
        {
            return Task.FromResult<Maybe>("Entity is inaccessible");
        }

        return Task.FromResult<Maybe>(true);
    }

    public override Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(GameState state, Character character, IInteractibleEntity target) =>
        Task.FromResult<Maybe<InteractionInstance>>(new HarvestInteractionInstance(character, this, target));
}
