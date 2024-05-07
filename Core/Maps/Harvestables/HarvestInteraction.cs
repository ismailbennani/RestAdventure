using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Kernel.Errors;

namespace RestAdventure.Core.Maps.Harvestables;

public class HarvestInteraction : Interaction
{
    public override string Name => "Harvest";

    public override Task<Maybe> CanInteractAsync(GameState state, Character character, IGameEntityWithInteractions entity)
    {
        if (entity is not HarvestableInstance harvestableInstance)
        {
            return Task.FromResult<Maybe>($"Expected {entity} to be a {nameof(HarvestableInstance)}, but got {entity.GetType()}");
        }

        if (character.Location != entity.Location)
        {
            return Task.FromResult<Maybe>("Entity is inaccessible");
        }

        if (harvestableInstance.Harvestable.HarvestCondition != null && !harvestableInstance.Harvestable.HarvestCondition.Evaluate(character))
        {
            return Task.FromResult<Maybe>("Character does not fulfill conditions");
        }

        return Task.FromResult<Maybe>(true);
    }

    public override Task<Maybe<InteractionInstance>> InstantiateInteractionAsync(GameState state, Character character, IGameEntityWithInteractions entity)
    {
        HarvestableInstance harvestableInstance = (HarvestableInstance)entity;
        return Task.FromResult<Maybe<InteractionInstance>>(new HarvestInteractionInstance(character, this, harvestableInstance));
    }
}
