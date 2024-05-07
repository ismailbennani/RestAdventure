using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Maps.Harvestables;

public class HarvestInteractionInstance : InteractionInstance
{
    long _startTick;

    internal HarvestInteractionInstance(Character character, HarvestInteraction interaction, HarvestableInstance harvestableInstance) : base(
        character,
        interaction,
        harvestableInstance
    )
    {
        HarvestableInstance = harvestableInstance;
    }

    public HarvestableInstance HarvestableInstance { get; }

    public override Task OnStartAsync(GameState state)
    {
        _startTick = state.Tick;
        return Task.CompletedTask;
    }

    public override bool IsOver(GameState state) => state.Tick - _startTick >= HarvestableInstance.Harvestable.HarvestDuration;

    public override Task OnEndAsync(GameState state)
    {
        Character.Inventory.Add(HarvestableInstance.Harvestable.Items);
        Character.Jobs.GainExperience(HarvestableInstance.Harvestable.Experience);
        return Task.CompletedTask;
    }
}
