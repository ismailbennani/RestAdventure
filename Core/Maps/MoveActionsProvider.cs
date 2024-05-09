using RestAdventure.Core.Actions;
using RestAdventure.Core.Entities.Characters;
using RestAdventure.Core.Maps.Locations;
using Action = RestAdventure.Core.Actions.Action;

namespace RestAdventure.Core.Maps;

public class MoveActionsProvider : IActionsProvider
{
    public IEnumerable<Action> GetActions(GameState state, Character character)
    {
        IEnumerable<Location> connectedLocations = state.Content.Maps.Locations.ConnectedTo(character.Location);
        foreach (Location location in connectedLocations)
        {
            yield return new MoveAction(location);
        }
    }
}
