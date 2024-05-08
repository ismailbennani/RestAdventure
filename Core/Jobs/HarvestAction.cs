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

    protected override Maybe CanPerformInternal(GameState state, Character character) => CanPerform(state, Job, Harvest, Target, character);

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

    public override string ToString() => $"{Harvest} | ${Target}";

    public static Maybe CanPerform(GameState state, Job job, JobHarvest harvest, StaticObjectInstance target, Character character)
    {
        JobInstance? jobInstance = character.Jobs.Get(job);

        if (jobInstance == null)
        {
            return "Unknown job";
        }

        if (harvest.Level > jobInstance.Progression.Level)
        {
            return "Job level too low";
        }

        if (!harvest.Match(target) || target.Busy)
        {
            return "Entity cannot be harvested";
        }

        return true;
    }
}
