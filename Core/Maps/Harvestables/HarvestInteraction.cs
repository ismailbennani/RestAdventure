using RestAdventure.Core.Characters;
using RestAdventure.Core.Interactions;

namespace RestAdventure.Core.Maps.Harvestables;

public class HarvestInteraction : Interaction
{
    public override string Name => "Harvest";

    public override bool CanInteract(Character character, IEntityWithInteractions entity)
    {
        if (entity is not HarvestableInstance harvestableInstance)
        {
            throw new InvalidOperationException($"Expected {entity} to be a {nameof(HarvestableInstance)}, but got {entity.GetType()}");
        }

        return harvestableInstance.Harvestable.HarvestCondition?.Evaluate(character) ?? true;
    }

    public override Task<InteractionInstance> InstantiateAsync(Character character, IEntityWithInteractions entity)
    {
        if (entity is not HarvestableInstance harvestableInstance)
        {
            throw new InvalidOperationException($"Expected {entity} to be a {nameof(HarvestableInstance)}, but got {entity.GetType()}");
        }

        return Task.FromResult((InteractionInstance)new HarvestInteractionInstance(character, this, harvestableInstance));
    }
}
