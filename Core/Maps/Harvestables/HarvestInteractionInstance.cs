using RestAdventure.Core.Characters;
using RestAdventure.Core.Gameplay.Interactions;
using RestAdventure.Core.Items;
using RestAdventure.Core.Jobs;

namespace RestAdventure.Core.Maps.Harvestables;

public class HarvestInteractionInstance : InteractionInstance
{
    long _startTick;

    public HarvestInteractionInstance(Character character, HarvestInteraction interaction, HarvestableInstance harvestableInstance) : base(
        character,
        interaction,
        harvestableInstance
    )
    {
        HarvestableInstance = harvestableInstance;
    }

    public HarvestableInstance HarvestableInstance { get; }

    public override Task OnStartAsync(GameContent content, GameState state)
    {
        _startTick = state.Tick;
        return Task.CompletedTask;
    }

    public override bool IsOver(GameContent content, GameState state) => state.Tick - _startTick >= HarvestableInstance.Harvestable.HarvestDuration;

    public override Task OnEndAsync(GameContent content, GameState state)
    {
        Character.Inventory.Add(HarvestableInstance.Harvestable.Items);
        Character.Jobs.GainExperience(HarvestableInstance.Harvestable.Experience);
        return Task.CompletedTask;
    }
}
