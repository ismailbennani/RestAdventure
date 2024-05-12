using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.StaticObjects;
using RestAdventure.Core.Items;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Jobs;

public class HarvestAction : Action
{
    public HarvestAction(Job job, JobHarvest harvest, StaticObjectInstanceId targetId) : base($"{harvest.Name}")
    {
        Job = job;
        Harvest = harvest;
        TargetId = targetId;
    }

    public Job Job { get; }
    public JobHarvest Harvest { get; }
    public StaticObjectInstanceId TargetId { get; }

    protected override Maybe CanPerformInternal(Game state, Character character)
    {
        JobInstance? jobInstance = character.Jobs.Get(Job);
        if (jobInstance == null)
        {
            return "Unknown job";
        }

        StaticObjectInstance? target = state.Entities.Get<StaticObjectInstance>(TargetId);
        if (target == null || !Harvest.CanTarget(target) || target.Busy)
        {
            return "Entity not found";
        }

        return jobInstance.CanHarvest(Harvest, target.Object);
    }

    public override bool IsOver(Game state, Character character) => state.Tick - StartTick >= Harvest.HarvestDuration;

    protected override Task OnStartAsync(Game state, Character character)
    {
        StaticObjectInstance target = state.Entities.Get<StaticObjectInstance>(TargetId)!;
        target.Busy = true;
        return Task.CompletedTask;
    }

    protected override async Task OnEndAsync(Game state, Character character)
    {
        character.Inventory.Add(Harvest.Items);
        character.Jobs.Get(Job)?.Progression.Progress(Harvest.Experience);

        StaticObjectInstance? target = state.Entities.Get<StaticObjectInstance>(TargetId);
        if (target != null)
        {
            await target.KillAsync(state);
        }
    }

    public override string ToString() => $"{Harvest} | {TargetId}";
}
