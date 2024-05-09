using RestAdventure.Core.Actions.Providers;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Entities.StaticObjects;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Jobs;

public class HarvestActionsProvider : IActionsProvider
{
    public IEnumerable<Action> GetActions(GameState state, Character character)
    {
        IEnumerable<StaticObjectInstance> staticObjects = state.Entities.AtLocation<StaticObjectInstance>(character.Location);
        foreach (StaticObjectInstance staticObject in staticObjects)
        foreach (JobInstance job in character.Jobs)
        foreach (JobHarvest harvest in job.Harvests)
        {
            if (!harvest.Match(staticObject))
            {
                continue;
            }

            yield return new HarvestAction(job.Job, harvest, staticObject);
        }
    }
}
