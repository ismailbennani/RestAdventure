using RestAdventure.Core.Characters;
using RestAdventure.Core.Interactions;
using RestAdventure.Core.Items;

namespace RestAdventure.Core.Jobs;

public class HarvestInteractionInstance : InteractionInstance
{
    long _startTick;

    internal HarvestInteractionInstance(Character character, HarvestInteraction interaction, IInteractibleEntity entity) : base(character, interaction, entity)
    {
        HarvestInteraction = interaction;
    }

    public HarvestInteraction HarvestInteraction { get; }

    public override Task OnStartAsync(GameState state)
    {
        _startTick = state.Tick;
        Target.Disable();
        return Task.CompletedTask;
    }

    public override bool IsOver(GameState state) => state.Tick - _startTick >= HarvestInteraction.Harvest.HarvestDuration;

    public override async Task OnEndAsync(GameState state)
    {
        Character.Inventory.Add(HarvestInteraction.Harvest.Items);
        Character.Jobs.Get(HarvestInteraction.Job)?.Progression.Progress(HarvestInteraction.Harvest.Experience);
        await Target.KillAsync(state);
    }
}
