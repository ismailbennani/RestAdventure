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
        JobInstance? job = character.Jobs.Get(Job);
        if (job == null || Harvest.Level > job.Progression.Level)
        {
            return "Character doesn't fulfill the conditions";
        }

        if (!Harvest.Targets.Contains(Target.Object))
        {
            return "Entity cannot be harvested";
        }

        return true;
    }

    public override bool IsOver(GameState state, Character character) => state.Tick - StartTick >= Harvest.HarvestDuration;

    protected override async Task OnEndAsync(GameState state, Character character)
    {
        character.Inventory.Add(Harvest.Items);
        character.Jobs.Get(Job)?.Progression.Progress(Harvest.Experience);

        await Target.KillAsync(state);
    }
}
