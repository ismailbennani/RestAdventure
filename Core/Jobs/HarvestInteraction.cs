using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.StaticObjects;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Jobs;

public class HarvestInteraction : Interaction
{
    public HarvestInteraction(Job job, JobHarvest harvest)
    {
        Name = job.Name + "-" + harvest.Name;
        Harvest = harvest;
    }

    public override string Name { get; }
    public JobHarvest Harvest { get; }

    public override Task<Maybe> CanInteractAsync(GameState state, Character character, IGameEntity entity)
    {
        if (entity is not StaticObjectInstance staticObjectInstance || !Harvest.Targets.Contains(staticObjectInstance.Object))
        {
            return Task.FromResult<Maybe>("Entity cannot be harvested");
        }

        if (character.Location != entity.Location)
        {
            return Task.FromResult<Maybe>("Entity is inaccessible");
        }

        return Task.FromResult<Maybe>(true);
    }

    public override Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(GameState state, Character character, IGameEntity entity) =>
        Task.FromResult<Maybe<InteractionInstance>>(new HarvestInteractionInstance(character, this, entity));
}
