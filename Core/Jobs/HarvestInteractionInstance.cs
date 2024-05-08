using RestAdventure.Core.Interactions;
using RestAdventure.Core.Items;

namespace RestAdventure.Core.Jobs;

public class HarvestInteractionInstance : InteractionInstance
{
    long _startTick;

    internal HarvestInteractionInstance(IGameEntityWithJobs sourceWithJobs, HarvestInteraction interaction, IInteractibleEntity entity) : base(
        (IInteractingEntity)sourceWithJobs,
        interaction,
        entity
    )
    {
        SourceWithJobs = sourceWithJobs;
        HarvestInteraction = interaction;
    }

    public IGameEntityWithJobs SourceWithJobs { get; }
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
        if (Source is IGameEntityWithInventory withInventory)
        {
            withInventory.Inventory.Add(HarvestInteraction.Harvest.Items);
        }

        SourceWithJobs.Jobs.Get(HarvestInteraction.Job)?.Progression.Progress(HarvestInteraction.Harvest.Experience);
        await Target.KillAsync(state);
    }
}
