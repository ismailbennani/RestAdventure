using RestAdventure.Core.Actions;
using RestAdventure.Core.Serialization;
using RestAdventure.Core.Serialization.Entities;
using RestAdventure.Core.Serialization.Jobs;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Jobs;

public class HarvestActionsProvider : IActionsProvider
{
    public IEnumerable<Action> GetActions(GameSnapshot state, CharacterSnapshot character)
    {
        IEnumerable<StaticObjectInstanceSnapshot> staticObjects = state.Entities.Values.OfType<StaticObjectInstanceSnapshot>().Where(o => o.Location == character.Location);
        foreach (StaticObjectInstanceSnapshot staticObject in staticObjects)
        foreach (JobInstanceSnapshot job in character.Jobs)
        foreach (JobHarvest harvest in job.Job.Harvests)
        {
            if (!harvest.CanTarget(staticObject))
            {
                continue;
            }

            yield return new HarvestAction(job.Job, harvest, staticObject.Id);
        }
    }
}
