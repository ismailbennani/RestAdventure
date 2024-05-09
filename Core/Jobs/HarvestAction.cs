using RestAdventure.Core.Characters;
using RestAdventure.Core.Items;
using RestAdventure.Core.StaticObjects;
using RestAdventure.Kernel.Errors;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Jobs;

public class HarvestAction : Action
{
    public HarvestAction(Job job, JobHarvest harvest, StaticObjectInstance target) : base($"{harvest.Name}")
    {
        Job = job;
        Harvest = harvest;
        Target = target;
    }

    public Job Job { get; }
    public JobHarvest Harvest { get; }
    public StaticObjectInstance Target { get; }

    protected override Maybe CanPerformInternal(GameState state, Character character)
    {
        if (character.Busy)
        {
            return "Character is busy";
        }

        JobInstance? jobInstance = character.Jobs.Get(Job);

        if (jobInstance == null)
        {
            return "Unknown job";
        }

        if (Harvest.Level > jobInstance.Progression.Level)
        {
            return "Job level too low";
        }

        if (!Harvest.Match(Target) || Target.Busy)
        {
            return "Entity cannot be harvested";
        }

        return true;
    }

    public override bool IsOver(GameState state, Character character) => state.Tick - StartTick >= Harvest.HarvestDuration;

    protected override Task OnStartAsync(GameState state, Character character)
    {
        Target.Busy = true;
        return Task.CompletedTask;
    }

    protected override async Task OnEndAsync(GameState state, Character character)
    {
        character.Inventory.Add(Harvest.Items);
        character.Jobs.Get(Job)?.Progression.Progress(Harvest.Experience);

        await Target.KillAsync(state);
    }

    public override string ToString() => $"{Harvest} | {Target}";
}
