using RestAdventure.Core.Characters;
using RestAdventure.Core.Entities;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Items;

namespace RestAdventure.Core.Jobs;

public class HarvestInteractionInstance : InteractionInstance
{
    long _startTick;

    internal HarvestInteractionInstance(Character character, HarvestInteraction interaction, IGameEntity entity) : base(character, interaction, entity)
    {
        HarvestInteraction = interaction;
    }

    public HarvestInteraction HarvestInteraction { get; }

    public override Task OnStartAsync(GameState state)
    {
        _startTick = state.Tick;
        return Task.CompletedTask;
    }

    public override bool IsOver(GameState state) => state.Tick - _startTick >= HarvestInteraction.Harvest.HarvestDuration;

    public override Task OnEndAsync(GameState state)
    {
        Character.Inventory.Add(HarvestInteraction.Harvest.Items);
        Character.Jobs.GainExperience(HarvestInteraction.Harvest.Experience);
        return Task.CompletedTask;
    }
}
