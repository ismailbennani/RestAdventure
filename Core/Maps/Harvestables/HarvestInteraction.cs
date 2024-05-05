using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Interactions;

namespace RestAdventure.Core.Maps.Harvestables;

public class HarvestInteraction : Interaction
{
    public override string Name => "Harvest";

    public override bool CanInteract(Character character, IGameEntityWithInteractions entity)
    {
        if (entity is not HarvestableInstance harvestableInstance)
        {
            throw new InvalidOperationException($"Expected {entity} to be a {nameof(HarvestableInstance)}, but got {entity.GetType()}");
        }

        return character.Location == entity.Location && (harvestableInstance.Harvestable.HarvestCondition?.Evaluate(character) ?? true);
    }

    public override InteractionInstance Instantiate(Character character, IGameEntityWithInteractions entity)
    {
        if (entity is not HarvestableInstance harvestableInstance)
        {
            throw new InvalidOperationException($"Expected {entity} to be a {nameof(HarvestableInstance)}, but got {entity.GetType()}");
        }

        return new HarvestInteractionInstance(character, this, harvestableInstance);
    }
}
